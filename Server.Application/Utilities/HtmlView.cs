using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server.Application.Utilities
{
    public enum HtmlTemplates
    {
        ResetPasswordForm,
        ConfirmationEmailView,
        ResetPasswordFail,
        ResetPassSucceedView
    }
    public class HtmlView
    {
        private ViewDataDictionary _viewData;
        public HtmlView(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }
        public ViewResult GetViewResult<T>(HtmlTemplates template, T obj = default(T))
        {
            var viewResult = new ViewResult()
            {
                ViewName = $"Views/{template.ToString()}.cshtml",
                ViewData = _viewData
            };
            if (!object.Equals(obj, default(T))) viewResult.ViewData.Model = (T)obj;
            return viewResult;
        }
    }
}