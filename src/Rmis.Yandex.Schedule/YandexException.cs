using System;

namespace Rmis.Yandex.Schedule
{
    [Serializable]
    public class YandexException : Exception
    {
        private readonly YandexError _error;

        public YandexException(YandexErrorResponse errorResponse)
        {
            _error = errorResponse?.error ?? throw new ArgumentNullException(nameof(errorResponse));
        }

        private string _message;

        public override string Message
        {
            get
            {
                if (_message == null)
                    _message = $"===Yandex.Api Exception=== {Environment.NewLine}Request: {_error.request} {Environment.NewLine}Text: {_error.text} {Environment.NewLine}Error Code: {_error.error_code} {Environment.NewLine}Http Code: {_error.http_code}";

                return _message;
            }
        }
    }
}