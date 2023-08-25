using System;
using System.Threading.Tasks;
using RecaptchaEnterprise.Models;

namespace RecaptchaEnterprise.Interfaces
{
	public interface IRecaptchaService
	{
        Task<RecaptchaResponse> InitializeRecaptch();
    }
}

