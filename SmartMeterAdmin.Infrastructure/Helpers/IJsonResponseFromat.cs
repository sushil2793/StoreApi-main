using System;

namespace SmartMeterAdmin.Infrastructure.Helpers
{
    public interface IJsonResponseFromat
    {
        Object JsonMessage(bool _success, string _message);
        Object JsonMessage(bool _success, Object model);
        Object JsonMessage(bool _success,string _message, Object model);
        Object JsonMessageWithID(bool _success, string _message, int id);
    }
    public class JsonResponseFromat : IJsonResponseFromat
    {
        public Object JsonMessage(bool _success, string _message)
        {
            var JsonResult = new
            {
                success = _success,
                message = _message

            };
            return JsonResult;
        }
        public Object JsonMessage(bool _success, Object model)
        {
            var JsonResult = new
            {
                success = _success,
                message = model

            };
            return JsonResult;
        }
        public Object JsonMessage(bool _success, string _message,Object model)
        {
            var JsonResult = new
            {
                success = _success,
                message = _message,
                data=model

            };
            return JsonResult;
        }
        public Object JsonMessageWithID(bool _success, string _message, int _id)
        {
            var JsonResult = new
            {
                success = _success,
                message = _message,
                id = _id
            };
            return JsonResult;
        }
    }

}
