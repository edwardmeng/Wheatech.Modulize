﻿using System;
using System.Runtime.Serialization;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The exception thrown when the modules or features configuration has an error.
    /// </summary>
    [Serializable]
    public class ModuleConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfigurationException"/> class.
        /// </summary>
        public ModuleConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfigurationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ModuleConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the  <see cref="ModuleConfigurationException"/> class 
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference (Nothing in Visual Basic), 
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public ModuleConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfigurationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination. 
        /// </param>
        protected ModuleConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}