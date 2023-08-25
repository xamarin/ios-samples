using System;

using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Google.Protobuf;


/*
 * missing libraries
using Google\Cloud\RecaptchaEnterprise\V1\AnnotateAssessmentRequest\Annotation;
using Google\Cloud\RecaptchaEnterprise\V1\AnnotateAssessmentResponse;
using Google\Cloud\RecaptchaEnterprise\V1\RecaptchaEnterpriseServiceClient;
*/ 

namespace RecaptchaEnterprise.Services
{
	public class RecaptchaEnterpriseAssestmentService
	{
        private const string _recaptchaSecretKey = "6LcqzrEnAAAAAG1ZeU6Fo5bWyVwskSlnPRnztcyE";

        private const string _projectName = "McAllister.iOS";

        public RecaptchaEnterpriseAssestmentService()
		{
 
        }



        /*public async Task<bool> VerifyRecaptchaAsync(string responseToken)
        {
            var client = await RecaptchaEnterpriseServiceClient.CreateAsync();

            var recaptchaRequest = new AnnotateAssessmentRequest
            {
                Assessment = new Assessment
                {
                    Event = new Event
                    {
                        SiteKey = RecaptchaSecretKey,
                        Token = responseToken
                    }
                }
            };

            try
            {
                var recaptchaResponse = await client.AnnotateAssessmentAsync(recaptchaRequest);
                return recaptchaResponse.Annotation == Annotation.Valid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying reCAPTCHA: {ex.Message}");
                return false;
            }
        }*/

        public async Task<bool> VerifyRecaptchaAsync(string responseToken)
        {
            var client = await RecaptchaEnterpriseServiceClient.CreateAsync();

            AnnotateAssessmentRequest recaptchaRequest = new AnnotateAssessmentRequest
            {
                //AssesssmentName = AssessmentName.FromProjectAssessment(_projectName, _recaptchaSecretKey),

                Annotation = AnnotateAssessmentRequest.Types.Annotation.Unspecified,

                Reasons =
                {
                    AnnotateAssessmentRequest.Types.Reason.Unspecified,
                },
                //HashedAccoundId = ByteString.Empty,
                TransactionEvent = new TransactionEvent(),

             };
            

            try
            {
                var recaptchaResponse = await client.AnnotateAssessmentAsync(recaptchaRequest);
                Console.WriteLine(recaptchaResponse.ToString());

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying reCAPTCHA: {ex.Message}");
                return false;
            }
        }
    }
}

