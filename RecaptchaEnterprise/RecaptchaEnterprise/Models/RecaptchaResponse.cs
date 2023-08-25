using System;
namespace RecaptchaEnterprise.Models
{
	public class RecaptchaResponse
	{
        public string FirstToken { get; set; }

        public string SecondToken { get; set; }

        public bool IsSuccess { get; set; }
    }
}

