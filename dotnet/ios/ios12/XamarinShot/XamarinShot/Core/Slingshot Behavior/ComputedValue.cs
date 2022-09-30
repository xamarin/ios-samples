namespace XamarinShot.Models;

using System;

// Using a provided closure the value is computed upon request.
// Managing classes can set the dirty state of the ComputedValue to
// force a consecutive compute of the value.

/// <summary>
/// Wrapper class for lazily computed values for use as properties elsewhere.
/// </summary>
public class ComputedValue<T>
{
        Func<T> compute;

        T? storage;

        public ComputedValue (Func<T> compute)
        {
                this.compute = compute;
        }

        // a flag specifying if the value has to be re-computed.
        // note: there is no way to set the flag to false from the outside
        public bool IsDirty { get; set; }

        /// <summary>
        /// accessor property to retrieved the computed value.
        /// </summary>
        public T Value
        {
                get
                {
                        ComputeIfRequired ();
                        return storage!;
                }
        }

        private void ComputeIfRequired ()
        {
                if (!IsDirty)
                {
                        return;
                }

                storage = compute ();
                IsDirty = false;
        }
}
