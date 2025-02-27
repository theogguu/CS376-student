﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Assets.Serialization
{
    /// <summary>
    /// Reconstructs a data structure from the output of Serializer that is equivalent to the data structure
    /// the Serializer started from.
    /// </summary>
    public class Deserializer
    {
        /// <summary>
        /// Reconstruct an object or set of objects from their serialization
        /// </summary>
        /// <param name="s">Serialization as a string</param>
        /// <returns>Copy of the object on which Serialize was originally called</returns>
        public static object Deserialize(string s) => Deserialize(new StringReader(s));

        /// <summary>
        /// Reconstruct an object or set of objects from their serialization
        /// </summary>
        /// <param name="r">TextReader for a stream containing the serialization</param>
        /// <returns>Copy of the object on which Serialize was originally called</returns>
        public static object Deserialize(TextReader r) => new Deserializer(r).ReadObject(-1);

        /// <summary>
        /// Make a Deserializer that can read the specified stream.
        /// Deserialize it by calling ReadObject(-1) on it.
        /// </summary>
        /// <param name="reader"></param>
        public Deserializer(TextReader reader)
        {
            this.reader = reader;
        }
        
        /// <summary>
        /// The stream from which we are reading.
        /// </summary>
        private readonly TextReader reader;

        /// <summary>
        /// True if we're at the end of the serialization data and there's nothing left to read.
        /// </summary>
        private bool End => reader.Peek() < 0;

        /// <summary>
        /// Returns the next character in the serialization data, without moving on the the next character.
        /// So if you call PeekChar over and over, you get the same character back until to call GetChar.
        /// </summary>
        private char PeekChar
        {
            get
            {
                var peek = reader.Peek();
                if (peek < 0)
                    throw new EndOfStreamException();
                return (char) peek;
            }
        }

        /// <summary>
        /// Gets the next character in the serialization.
        /// This will return the same character as PeekChar, but this is also move on to the next character
        /// so that subsequent calls will return new data.
        /// </summary>
        /// <returns>The current cahracter</returns>
        private char GetChar()
        {
            var next = reader.Read();
            if (next < 0)
                throw new EndOfStreamException();
            return (char)next;
        }

        /// <summary>
        /// Skips over any spaces, tabs, or newlines, if any.
        /// After calling this, PeekChar will be the next next non-whitespace character.
        /// </summary>
        private void SkipWhitespace()
        {
            while (!End && char.IsWhiteSpace(PeekChar))
                GetChar();
        }

        /// <summary>
        /// Reads the next characters until it finds a character that can't be part of a number or a word.
        /// </summary>
        /// <returns></returns>
        private string ReadToken()
        {
            bool InsideToken()
            {
                var p = reader.Peek();
                if (p < 0)
                    return false;
                var c = (char) p;
                return char.IsLetterOrDigit(c) || c == '.' || c == '-' ;
            }

            var b = new StringBuilder();

            while (InsideToken())
                b.Append(GetChar());

            return b.ToString();
        }

        /// <summary>
        /// Reads a null or Boolean from the input.
        /// Generates an exception if the next token in the input is not in fact "null", "True", or "False".
        /// </summary>
        /// <param name="enclosingId">Object id of the object we're currently trying to read.  This is just used for debugging messages.</param>
        /// <returns></returns>
        private object ReadSpecialName(int enclosingId)
        {
            var token = ReadToken();

            switch (token)
            {
                case "null":
                    return null;

                case "True":
                    return true;

                case "False":
                    return false;

                default:
                    throw new Exception($"Unknown token found while reading object id {enclosingId}: {token}");
            }
        }

        /// <summary>
        /// Returns the next number in the input.
        /// Generates an exception if the next characters are not in fact a number.
        /// </summary>
        /// <param name="enclosingId"></param>
        /// <returns></returns>
        private object ReadNumber(int enclosingId)
        {
            var token = ReadToken();
            if (int.TryParse(token, out var i))
                return i;
            if (float.TryParse(token, out var f))
                return f;
            throw new Exception($"Unknown number format while reading object {enclosingId}: {token}");
        }

        /// <summary>
        /// Reads a string value from the input.
        /// The next character must be a quote.  It return all the characters between it and the following quote.
        /// TODO: this doesn't understand backslashes to escape quotes in a string.
        /// </summary>
        /// <param name="enclosingId"></param>
        /// <returns></returns>
        private object ReadString(int enclosingId)
        {
            GetChar();  // Swallow the quote
            var b = new StringBuilder();
            while (!End && PeekChar != '"')
                b.Append(GetChar());
            if (End)
                throw new EndOfStreamException($"Data ended in the middle of a string while reading object {enclosingId}!");
            GetChar();  // Swallow the quote
            return b.ToString();
        }

        /// <summary>
        /// Reads a list represented in [ ] format and returns its data as an object list.
        /// </summary>
        /// <param name="enclosingId">id of the object wer'e currently reading, for inclusion in any error messages</param>
        private List<object> ReadList(int enclosingId)
        {
            var result = new List<object>();

            GetChar();  // Swallow open bracket
            SkipWhitespace();
            while (!End && PeekChar != ']')
            {
                result.Add(ReadObject(enclosingId));
                SkipWhitespace();
                if (End)
                    throw new EndOfStreamException($"End of stream in the middle of a list while reading object id {enclosingId}");
                var c = PeekChar;
                if (c != ',' && c != ']')
                    throw new Exception($"Expected comma between list elements, but got {c} while reading object id {enclosingId}");
                if (c == ',')
                    GetChar();   // Swallow comma
                SkipWhitespace();
            }

            if (End)
                throw new EndOfStreamException($"Stream ended in the middle of a list while reading object id {enclosingId}");

            GetChar(); // Swallow close bracket
            return result;
        }

        /// <summary>
        /// Reads an object field in the format "fieldName: value", and returns both the fieldName and the value.
        /// </summary>
        /// <param name="enclosingId">Object id of the object currently being read, for inclusion in error messages.</param>
        /// <returns>A tuple of the field's name and the field's value</returns>
        private (string fieldName, object value) ReadField(int enclosingId)
        {
            SkipWhitespace();
            var fieldName = ReadToken();
            if (End)
                throw new Exception($"Stream ended unexpectedly after field name {fieldName} in object id {enclosingId}");
            var c = GetChar();
            if (c != ':')
                throw new Exception($"Expected a colon after {fieldName} in object id {enclosingId}");
            SkipWhitespace();
            if (End)
                throw new Exception($"Stream ended unexpectedly after field name {fieldName} in object id {enclosingId}");
            var value = ReadObject(enclosingId);

            SkipWhitespace();
            if (!End && PeekChar == ',')
            {
                GetChar();
                SkipWhitespace();
            }
            return (fieldName, value);
        }

        /// <summary>
        /// Table of previously deserialized objects..
        /// idTable[id] is the object that was previously deserialized under the specified id.
        /// </summary>
        private readonly Dictionary<int, object> idTable = new Dictionary<int, object>();

        /// <summary>
        /// Read whatever data object is next in the stream
        /// </summary>
        /// <param name="enclosingId">The object id of whatever object this is a part of, if any</param>
        /// <returns>The deserialized object</returns>
        public object ReadObject(int enclosingId)
        {
            SkipWhitespace();

            if (End)
                throw new EndOfStreamException();

            switch (PeekChar)
            {
                case '#':
                    return ReadComplexObject(enclosingId);

                case '[':
                    return ReadList(enclosingId);

                case '"':
                    return ReadString(enclosingId);

                case '-':
                case '.':
                case var c when char.IsDigit(c):
                    return ReadNumber(enclosingId);

                case var c when char.IsLetter(c):
                    return ReadSpecialName(enclosingId);

                default:
                    throw new Exception($"Unexpected character {PeekChar} found while reading object id {enclosingId}");
            }
        }

        /// <summary>
        /// Called when the next character is a #.  Read the object id of the object and return the
        /// object.  If that object id has already been read, return the object previously returned.
        /// Otherwise, there will be a { } expression after the object id.  Read it, create the object
        /// it represents, and return it.
        /// </summary>
        /// <param name="enclosingId">Object id of the object this expression appears inside of, if any.</param>
        /// <returns>The object referred to by this #id expression.</returns>
        private object ReadComplexObject(int enclosingId)
        {
            GetChar();  // Swallow the #
            int id = (int)ReadNumber(enclosingId);
            SkipWhitespace();

            // You've got the id # of the object.  Are we done now?
            if (idTable.ContainsKey(id))
            {
                return idTable[id]; // access and return
            }

            // Assuming we aren't done, let's check to make sure there's a { next
            SkipWhitespace();
            if (End)
                throw new EndOfStreamException($"Stream ended after reference to unknown ID {id}");
            char c = GetChar();
            if (c != '{')
   
                throw new Exception($"Expected '{'{'}' after #{id} but instead got {c}");

            // There's a {.
            // Let's hope there's a type: typename line.
            var (hopefullyType, typeName) = ReadField(id);
            if (hopefullyType != "type")
                throw new Exception(
                    $"Expected type name at the beginning of complex object id {id} but instead got {typeName}");
            string type = typeName as string;
            if (type == null)
                throw new Exception(
                    $"Expected a type name (a string) in 'type: ...' expression for object id {id}, but instead got {typeName}");

            // Great!  Now what?
            var obj = Utilities.MakeInstance(type); // new obj without fields
            idTable[id] = obj; // put object to dictionary

            // Read the fields until we run out of them
            while (!End && PeekChar != '}')
            {
                var (field, value) = ReadField(id);
                // We've got a field and a value.  Now what?
                Utilities.SetFieldByName(obj, field, value);
            }

            if (End)
                throw new EndOfStreamException($"Stream ended in the middle of {"{ }"} expression for id #{id}");

            GetChar();  // Swallow close bracket

            // We're done.  Now what
            return obj; // return object
        }

    }
}
