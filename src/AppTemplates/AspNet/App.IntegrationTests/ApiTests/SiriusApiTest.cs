using Altinn.App.Common.Enums;
using Altinn.App.IntegrationTests;
using Altinn.App.Services.Models.Validation;
using Altinn.Common.PEP.Models;
using Altinn.Platform.Storage.Interface.Models;
using App.IntegrationTests.Utils;
using App.IntegrationTestsRef.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using App.IntegrationTests.Mocks.Apps.tdd.sirius;
using System.Xml.Serialization;

namespace App.IntegrationTests
{
    public class SiriusApiTest: IClassFixture<CustomWebApplicationFactory<Altinn.App.Startup>>
    {
        private readonly CustomWebApplicationFactory<Altinn.App.Startup> _factory;

        public SiriusApiTest(CustomWebApplicationFactory<Altinn.App.Startup> factory)
        {
            _factory = factory;
        }




        [Fact]
        public async Task Instance_Post_InstansiateWithoutAddingData()
        {
            string token = PrincipalUtil.GetToken(1337);

            HttpClient client = SetupUtil.GetTestClient(_factory, "tdd", "sirius");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/tdd/sirius/instances?instanceOwnerPartyId=1337")
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = response.Content.ReadAsStringAsync().Result;
            Instance instance = JsonConvert.DeserializeObject<Instance>(responseContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(instance);
            Assert.Equal("1337", instance.InstanceOwner.PartyId);

            TestDataUtil.DeletInstanceAndData("tdd", "sirius", 1337, new Guid(instance.Id.Split('/')[1]));
        }

        /// <summary>
        /// This test verifies that you can instansiate sirius app and add næringsoppgave and skattemdling
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Instance_Post_WithNæringOgSkattemelding_ValidateOk()
        {
            string token = PrincipalUtil.GetToken(1337);

            HttpClient client = SetupUtil.GetTestClient(_factory, "tdd", "sirius");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/tdd/sirius/instances?instanceOwnerPartyId=1337")
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = response.Content.ReadAsStringAsync().Result;
            Instance instance = JsonConvert.DeserializeObject<Instance>(responseContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(instance);
            Assert.Equal("1337", instance.InstanceOwner.PartyId);


            // Get Data from Instance
            HttpRequestMessage httpRequestMessageGetData = new HttpRequestMessage(HttpMethod.Get, "/tdd/sirius/instances/" + instance.Id + "/data/" + instance.Data[0].Id);
            {
            };

            HttpResponseMessage responseData = await client.SendAsync(httpRequestMessageGetData);
            string responseContentData = responseData.Content.ReadAsStringAsync().Result;


            Skjema siriusMainForm = (Skjema)JsonConvert.DeserializeObject(responseContentData, typeof(Skjema));

            // Modify the prefilled form. This would need to be replaced with the real sirius form
            siriusMainForm.Permisjonsopplysningergrp8822 = new Permisjonsopplysningergrp8822();
            siriusMainForm.Permisjonsopplysningergrp8822.AnsattEierandel20EllerMerdatadef33294 = new AnsattEierandel20EllerMerdatadef33294() { value = "50" };

            XmlSerializer serializer = new XmlSerializer(typeof(Skjema));
            using MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, siriusMainForm);
            stream.Position = 0;
            StreamContent streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage putresponse = await client.PutAsync("/tdd/sirius/instances/" + instance.Id + "/data/" + instance.Data[0].Id, streamContent);

            // Add Næringsoppgave.xml
            string næringsppgave = File.ReadAllText("Data/Files/data-element.xml");

            byte[] byteArray = Encoding.UTF8.GetBytes(næringsppgave);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream næringsoppgavestream = new MemoryStream(byteArray);

            StreamContent streamContentNæring = new StreamContent(næringsoppgavestream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage postresponseNæring = await client.PostAsync("/tdd/sirius/instances/" + instance.Id + "/data/?datatype=næringsoppgave", streamContentNæring);
            DataElement dataElementNæring = (DataElement)JsonConvert.DeserializeObject(postresponseNæring.Content.ReadAsStringAsync().Result, typeof(DataElement));


            // Add skattemelding.xml
            string skattemelding = File.ReadAllText("Data/Files/data-element.xml");

            byte[] byteArraySkattemelding = Encoding.UTF8.GetBytes(næringsppgave);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream skattemeldingstream = new MemoryStream(byteArraySkattemelding);

            StreamContent streamContentSkattemelding = new StreamContent(skattemeldingstream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage postresponseskattemelding = await client.PostAsync("/tdd/sirius/instances/" + instance.Id + "/data/?datatype=skattemelding", streamContentNæring);
            DataElement dataElementSkattemelding = (DataElement)JsonConvert.DeserializeObject(postresponseskattemelding.Content.ReadAsStringAsync().Result, typeof(DataElement));

            // Validate instance
            string url = "/tdd/sirius/instances/" + instance.Id + "/validate";
            HttpResponseMessage responseValidation = await client.GetAsync(url);
            string responseContentValidation = responseValidation.Content.ReadAsStringAsync().Result;
            List<ValidationIssue> messages = (List<ValidationIssue>)JsonConvert.DeserializeObject(responseContentValidation, typeof(List<ValidationIssue>));

            Assert.Empty(messages);
            TestDataUtil.DeletInstanceAndData("tdd", "sirius", 1337, new Guid(instance.Id.Split('/')[1]));
        }


        /// <summary>
        /// This test verifies that you can instansiate sirius app and add næringsoppgave and skattemdling
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Instance_Post_WithNæringOgSkattemelding_MissingSkattemelding()
        {
            string token = PrincipalUtil.GetToken(1337);

            HttpClient client = SetupUtil.GetTestClient(_factory, "tdd", "sirius");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/tdd/sirius/instances?instanceOwnerPartyId=1337")
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = response.Content.ReadAsStringAsync().Result;
            Instance instance = JsonConvert.DeserializeObject<Instance>(responseContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(instance);
            Assert.Equal("1337", instance.InstanceOwner.PartyId);


            // Get Data from Instance
            HttpRequestMessage httpRequestMessageGetData = new HttpRequestMessage(HttpMethod.Get, "/tdd/sirius/instances/" + instance.Id + "/data/" + instance.Data[0].Id);
            {
            };

            HttpResponseMessage responseData = await client.SendAsync(httpRequestMessageGetData);
            string responseContentData = responseData.Content.ReadAsStringAsync().Result;


            Skjema siriusMainForm = (Skjema)JsonConvert.DeserializeObject(responseContentData, typeof(Skjema));

            // Modify the prefilled form. This would need to be replaced with the real sirius form
            siriusMainForm.Permisjonsopplysningergrp8822 = new Permisjonsopplysningergrp8822();
            siriusMainForm.Permisjonsopplysningergrp8822.AnsattEierandel20EllerMerdatadef33294 = new AnsattEierandel20EllerMerdatadef33294() { value = "50" };

            XmlSerializer serializer = new XmlSerializer(typeof(Skjema));
            using MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, siriusMainForm);
            stream.Position = 0;
            StreamContent streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage putresponse = await client.PutAsync("/tdd/sirius/instances/" + instance.Id + "/data/" + instance.Data[0].Id, streamContent);

            // Add Næringsoppgave.xml
            string næringsppgave = File.ReadAllText("Data/Files/data-element.xml");

            byte[] byteArray = Encoding.UTF8.GetBytes(næringsppgave);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream næringsoppgavestream = new MemoryStream(byteArray);

            StreamContent streamContentNæring = new StreamContent(næringsoppgavestream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage postresponseNæring = await client.PostAsync("/tdd/sirius/instances/" + instance.Id + "/data/?datatype=næringsoppgave", streamContentNæring);
            DataElement dataElementNæring = (DataElement)JsonConvert.DeserializeObject(postresponseNæring.Content.ReadAsStringAsync().Result, typeof(DataElement));

            //Validate instance
            string url = "/tdd/sirius/instances/" + instance.Id + "/validate";
            HttpResponseMessage responseValidation = await client.GetAsync(url);
            string responseContentValidation = responseValidation.Content.ReadAsStringAsync().Result;
            List<ValidationIssue> messages = (List<ValidationIssue>)JsonConvert.DeserializeObject(responseContentValidation, typeof(List<ValidationIssue>));

            Assert.Single(messages);
            TestDataUtil.DeletInstanceAndData("tdd", "sirius", 1337, new Guid(instance.Id.Split('/')[1]));
        }



        /// <summary>
        /// This test to the following.
        /// It instansiates a new instance of app/sirus for party with id 1337
        /// It then upload both næringsmelding.xml an d
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Instance_Post_WithNæringOgSkattemelding_ValidateOkThenNext()
        {
            // Gets JWT token. In production this would be given from authentication component when exchanging a ID porten token.
            string token = PrincipalUtil.GetToken(1337);


            // Setup client and calls Instance controller on the App that instansiates a new instances of the app
            HttpClient client = SetupUtil.GetTestClient(_factory, "tdd", "sirius");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/tdd/sirius/instances?instanceOwnerPartyId=1337")
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = response.Content.ReadAsStringAsync().Result;
            Instance instance = JsonConvert.DeserializeObject<Instance>(responseContent);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(instance);
            Assert.Equal("1337", instance.InstanceOwner.PartyId);


            // Get Data from the main form
            HttpRequestMessage httpRequestMessageGetData = new HttpRequestMessage(HttpMethod.Get, "/tdd/sirius/instances/" + instance.Id + "/data/" + instance.Data[0].Id);
            {
            };

            HttpResponseMessage responseData = await client.SendAsync(httpRequestMessageGetData);
            string responseContentData = responseData.Content.ReadAsStringAsync().Result;
            Skjema siriusMainForm = (Skjema)JsonConvert.DeserializeObject(responseContentData, typeof(Skjema));

            // Modify the prefilled form. This would need to be replaced with the real sirius form
            siriusMainForm.Permisjonsopplysningergrp8822 = new Permisjonsopplysningergrp8822();
            siriusMainForm.Permisjonsopplysningergrp8822.AnsattEierandel20EllerMerdatadef33294 = new AnsattEierandel20EllerMerdatadef33294() { value = "50" };

            XmlSerializer serializer = new XmlSerializer(typeof(Skjema));
            using MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, siriusMainForm);
            stream.Position = 0;
            StreamContent streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage putresponse = await client.PutAsync("/tdd/sirius/instances/" + instance.Id + "/data/" + instance.Data[0].Id, streamContent);

            // Add Næringsoppgave.xml
            string næringsppgave = File.ReadAllText("Data/Files/data-element.xml");

            byte[] byteArray = Encoding.UTF8.GetBytes(næringsppgave);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream næringsoppgavestream = new MemoryStream(byteArray);

            StreamContent streamContentNæring = new StreamContent(næringsoppgavestream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage postresponseNæring = await client.PostAsync("/tdd/sirius/instances/" + instance.Id + "/data/?datatype=næringsoppgave", streamContentNæring);
            DataElement dataElementNæring = (DataElement)JsonConvert.DeserializeObject(postresponseNæring.Content.ReadAsStringAsync().Result, typeof(DataElement));


            // Add skattemelding.xml
            string skattemelding = File.ReadAllText("Data/Files/data-element.xml");

            byte[] byteArraySkattemelding = Encoding.UTF8.GetBytes(skattemelding);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream skattemeldingstream = new MemoryStream(byteArraySkattemelding);

            StreamContent streamContentSkattemelding = new StreamContent(skattemeldingstream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            HttpResponseMessage postresponseskattemelding = await client.PostAsync("/tdd/sirius/instances/" + instance.Id + "/data/?datatype=skattemelding", streamContentNæring);
            DataElement dataElementSkattemelding = (DataElement)JsonConvert.DeserializeObject(postresponseskattemelding.Content.ReadAsStringAsync().Result, typeof(DataElement));

            // Validate instance. This validates that main form has valid data and required data 
            string url = "/tdd/sirius/instances/" + instance.Id + "/validate";
            HttpResponseMessage responseValidation = await client.GetAsync(url);
            string responseContentValidation = responseValidation.Content.ReadAsStringAsync().Result;
            List<ValidationIssue> messages = (List<ValidationIssue>)JsonConvert.DeserializeObject(responseContentValidation, typeof(List<ValidationIssue>));

            Assert.Empty(messages);

            // Handle first next go from data to confirmation
            HttpRequestMessage httpRequestMessageFirstNext = new HttpRequestMessage(HttpMethod.Put, "/tdd/sirius/instances/" + instance.Id + "/process/next")
            {
            };

            HttpResponseMessage responseFirstNext = await client.SendAsync(httpRequestMessageFirstNext);
            string responseContentFirstNext = responseFirstNext.Content.ReadAsStringAsync().Result;

            ProcessState stateAfterFirstNext = (ProcessState)JsonConvert.DeserializeObject(responseContentFirstNext, typeof(ProcessState));
            Assert.Equal("Task_2",stateAfterFirstNext.CurrentTask.ElementId);

            // Validate instance in Task_2. This validates that PDF for nærings is in place
            HttpResponseMessage responseValidationTask2 = await client.GetAsync(url);
            string responseContentValidationTask2 = responseValidationTask2.Content.ReadAsStringAsync().Result;
            List<ValidationIssue> messagesTask2 = (List<ValidationIssue>)JsonConvert.DeserializeObject(responseContentValidationTask2, typeof(List<ValidationIssue>));
            Assert.Empty(messagesTask2);

            // Move process from Task_2 (Confirmation) to Task_3  (Feedback). 
            HttpRequestMessage httpRequestMessageSecondNext = new HttpRequestMessage(HttpMethod.Put, "/tdd/sirius/instances/" + instance.Id + "/process/next")
            {
            };

            HttpResponseMessage responseSecondNext = await client.SendAsync(httpRequestMessageSecondNext);
            string responseContentSecondNext = responseSecondNext.Content.ReadAsStringAsync().Result;

            ProcessState stateAfterSecondNext = (ProcessState)JsonConvert.DeserializeObject(responseContentSecondNext, typeof(ProcessState));
            Assert.Equal("Task_3", stateAfterSecondNext.CurrentTask.ElementId);

            // Delete all data created
            TestDataUtil.DeletInstanceAndData("tdd", "sirius", 1337, new Guid(instance.Id.Split('/')[1]));
        }
    }
}
