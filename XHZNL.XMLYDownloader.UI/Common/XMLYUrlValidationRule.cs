using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XHZNL.XMLYDownloader.UI.Common
{
    /// <summary>
    /// ximalaya url验证
    /// </summary>
    public class XMLYUrlValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var r = CommonHelper.Instance.XMLYUrlValidation(value.ToString());

                return r ? ValidationResult.ValidResult : new ValidationResult(false, "资源未识别");
            }
            catch
            {
                return new ValidationResult(false, "资源未识别");
            }
        }
    }
}
