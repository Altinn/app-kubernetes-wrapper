using System;
using System.Threading.Tasks;

using Altinn.App.Common.Enums;
using Altinn.App.Common.Models;
using Altinn.App.Services.Implementation;
using Altinn.App.Services.Interface;
using Altinn.Platform.Storage.Interface.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace App.IntegrationTests.Mocks.Apps.tdd.platform_fails
{
    public class AltinnApp : AppBase, IAltinnApp
    {

        public AltinnApp(IAppResources appResourcesService, ILogger<AltinnApp> logger, IData dataService, IProcess processService, IPDF pdfService, IPrefill prefillService) : base(appResourcesService, logger, dataService, processService, pdfService, prefillService)
        {
        }

        public override object CreateNewAppModel(string classRef)
        {
            Type appType = Type.GetType(classRef);
            return Activator.CreateInstance(appType);
        }

        public override Type GetAppModelType(string classRef)
        {
            return Type.GetType(classRef);
        }

        public override Task<bool> RunAppEvent(AppEventType appEvent, object model, ModelStateDictionary modelState = null)
        {
            return Task.FromResult(true);
        }

        public override async Task RunDataValidation(object data, ModelStateDictionary validationResults)
        {
            await Task.CompletedTask;
        }

        public override async Task RunTaskValidation(Instance instance, string taskId, ModelStateDictionary validationResults)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Run validation event to perform custom validations
        /// </summary>
        /// <param name="validationResults">Object to contain any validation errors/warnings</param>
        /// <returns>Value indicating if the form is valid or not</returns>
        public override Task<bool> RunCalculation(object data)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Run validation event to perform custom validations
        /// </summary>
        /// <param name="validationResults">Object to contain any validation errors/warnings</param>
        /// <returns>Value indicating if the form is valid or not</returns>
        public override async Task<Altinn.App.Services.Models.Validation.InstantiationValidationResult> RunInstantiationValidation(Instance instance)
        {
            await Task.CompletedTask;
            return null;
        }

        /// <summary>
        /// Is called to run custom instantiation events defined by app developer.
        /// </summary>
        /// <remarks>
        /// Instantiation events include validation and data manipulation (custom prefill)
        /// </remarks>
        /// <param name="instance">The data to perform calculations on</param>
        /// <param name="validationResults">Object containing any validation errors/warnings</param>
        /// <returns>Task to indicate when calculation is completed</returns>
        public override async Task RunDataCreation(Instance instance, object data)
        {
            await Task.CompletedTask;
        }

        public override Task<AppOptions> GetOptions(string id, AppOptions options)
        {
            return Task.FromResult(options);
        }
    }
}
