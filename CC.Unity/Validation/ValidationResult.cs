using System.Collections.Generic;
using UnityEngine;

namespace CC.Unity.Validation
{
    internal class ValidationResult
    {
        internal enum Level
        {
            Pass    = 0,
            Warning = 1,
            Fail    = 2
        }

        private Level _level;
        private List<string> _warnings;
        private List<string> _errors;

        public bool CanExport => _level <= Level.Warning;
        public bool RequireConfirm => _level == Level.Warning;
        public bool Failed => _level >= Level.Fail;

        public ValidationResult()
        {
            _level = Level.Pass;
            _warnings = new List<string>();
            _errors = new List<string>();
        }

        public ValidationResult ScaleToWarning(string message)
        {
            if (_level < Level.Warning)
            {
                _level = Level.Warning;
            }

            if (!string.IsNullOrEmpty(message))
            {
                _warnings.Add(message);
            }

            return this;
        }

        public ValidationResult ScaleToFailure(string message)
        {
            if (_level < Level.Fail)
            {
                _level = Level.Fail;
            }

            if (!string.IsNullOrEmpty(message))
            {
                _errors.Add(message);
            }

            return this;
        }

        public void Log()
        {
            foreach (var item in _warnings)
            {
                Debug.LogWarning(item);
            }

            foreach (var item in _errors)
            {
                Debug.LogError(item);
            }
        }
    }
}
