using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Utils.Implementation
{
    public static class ApplicationExceptionHandler
    {

        /// <summary>
        /// Valida un IEnumerable per verificare se è null o vuoto e lancia le eccezioni appropriate
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi nella collezione</typeparam>
        /// <param name="collection">La collezione da validare</param>
        /// <param name="className">Nome della classe chiamante</param>
        /// <param name="methodName">Nome del metodo chiamante</param>
        /// <param name="expectEmpty">Se true, si aspetta che la lista possa essere vuota</param>
        /// <param name="customMessage">Messaggio personalizzato per l'eccezione</param>
        /// <exception cref="ArgumentNullException">Se la collezione è null</exception>
        /// <exception cref="EmptyListExeption">Se la collezione è vuota e non dovrebbe esserlo</exception>
        /// <exception cref="ExpectedEmptyListException">Se la collezione ci si aspetta possa essere vuota</exception>
        public static void ValidateCollection<T>(
            IEnumerable<T> collection, 
            string className, 
            string methodName, 
            bool expectEmpty = false, 
            string customMessage = "")
        {
            // Controllo null
            if (collection == null)
            {
                string message = customMessage != "" ? customMessage : "La collezione non può essere null";
                throw new ArgumentNullException(nameof(collection), message);
            }

            // Materializza la collezione una sola volta per evitare multiple enumerazioni
            var materializedCollection = collection.ToList();

            if (expectEmpty)
            {
                // Si aspetta che la lista sia vuota
                if (!materializedCollection.Any())
                {
                    return;
                }
            }
            else
            {
                // Non ci si aspetta che la lista sia vuota
                if (!materializedCollection.Any())
                {
                    string message = customMessage != "" ? customMessage : "La collezione non può essere vuota";
                    throw new EmptyListException(className, methodName, message);
                }
            }
        }

        /// <summary>
        /// Overload semplificato per il caso più comune (collezione non deve essere null o vuota)
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi nella collezione</typeparam>
        /// <param name="collection">La collezione da validare</param>
        /// <param name="className">Nome della classe chiamante</param>
        /// <param name="methodName">Nome del metodo chiamante</param>
        public static void ValidateNotNullOrEmptyList<T>(
            IEnumerable<T> collection, 
            string className, 
            string methodName)
        {
            ValidateCollection(collection, className, methodName, expectEmpty: false);
        }

        /// <summary>
        /// Overload per validare che la collezione possa essere vuota
        /// </summary>
        /// <typeparam name="T">Tipo degli elementi nella collezione</typeparam>
        /// <param name="collection">La collezione da validare</param>
        /// <param name="className">Nome della classe chiamante</param>
        /// <param name="methodName">Nome del metodo chiamante</param>
        public static void ValidateEmptyList<T>(
            IEnumerable<T> collection, 
            string className, 
            string methodName)
        {
            ValidateCollection(collection, className, methodName, expectEmpty: true);
        }
    }

    public class DatabaseExeption : Exception
    {
        public string MethodName { get; set; }
        public string ClassName { get; set; }

        public DatabaseExeption(string className, string methodName, string message) : base(message)
        {
            MethodName = methodName;
            ClassName = className;
        }

        public DatabaseExeption(string className, string methodName, string message, Exception inner) : base(message, inner)
        {
            MethodName = methodName;
            ClassName = className;
        }
    }

    public class EmptyListException : Exception
    {
        public string MethodName { get; set; }
        public string ClassName { get; set; }

        public EmptyListException(string className, string methodName, string message) : base(message)
        {
            MethodName = methodName;
            ClassName = className;
        }
    }

    // Ci si aspetta che la lista possa essere vuota. Lancia l'eccezione per segnalare un NoContent.
    // Questo viene fatto per gestire sia i casi in cui la lista è vuota ma è un risultato atteso,
    // sia per evitare di lanciare un'eccezione di tipo EmptyListExeption quando la lista è vuota.
    public class ExpectedEmptyListException : Exception
    {
        public string MethodName { get; set; }
        public string ClassName { get; set; }

        public ExpectedEmptyListException(string className, string methodName, string message) : base(message)
        {
            MethodName = methodName;
            ClassName = className;
        }
    }
}