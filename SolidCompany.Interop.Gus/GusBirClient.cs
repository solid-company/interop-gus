﻿using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using SolidCompany.Interop.Gus.Bir;
using SolidCompany.Interop.Gus.Connected_Services;
using SolidCompany.Interop.Gus.Models;
using SolidCompany.Interop.Gus.Models.ErrorDeserialization;
using SolidCompany.Interop.Gus.Models.Generated;
using WcfCoreMtomEncoder;

namespace SolidCompany.Interop.Gus
{
    /// <inheritdoc cref="IGusBirClient" />
    public sealed class GusBirClient : IGusBirClient, IDisposable
#if NET5_0_OR_GREATER
    , IAsyncDisposable
#endif
    {
        private readonly UslugaBIRzewnPublClient webServiceClient;

        private string sessionKey;

        private readonly Task loginTask;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">User key</param>
        /// <param name="environment">GUS environment</param>
        /// <param name="proxyAddress">Proxy server address</param>
        /// <exception cref="ArgumentNullException">Thrown when key or environment is not specified.</exception>
        public GusBirClient(string key, GusEnironment environment, Uri proxyAddress = null)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key), "Key not specified.");
            _ = environment ?? throw new ArgumentNullException(nameof(environment), "Environment not specified.");

            var mtomMessageEncoderBindingElement = new MtomMessageEncoderBindingElement(new TextMessageEncodingBindingElement());
            var httpsBindingElement = new HttpsTransportBindingElement();
            httpsBindingElement.ProxyAddress = proxyAddress;

            var customBinding = new CustomBinding(mtomMessageEncoderBindingElement, httpsBindingElement);

            webServiceClient = new UslugaBIRzewnPublClient(customBinding, new EndpointAddress(environment.ServiceUrl));

            loginTask = LogInAsync(key);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">GusBirClient options - <see cref="GusBirOptions"/></param>
        public GusBirClient(IOptions<GusBirOptions> options)
            : this(options.Value.Key, options.Value.Environment, options.Value.ProxyAddress)
        {
        }

        private async Task LogInAsync(string key)
        {
            var response = await webServiceClient.ZalogujAsync(key);

            if (response.ZalogujResult?.Length != 20)
                throw new InvalidOperationException("Login failed.");

            sessionKey = response.ZalogujResult;
        }

        /// <inheritdoc />
        public async Task<SearchResult> FindByNipAsync(string nip)
        {
            await loginTask;

            var parametryWyszukiwania = new ParametryWyszukiwania
            {
                Nip = nip
            };

            return await GetLegalEntityAsync(parametryWyszukiwania);
        }

        /// <inheritdoc />
        public async Task<SearchResult> FindByRegonAsync(string regon)
        {
            await loginTask;

            var parametryWyszukiwania = new ParametryWyszukiwania
            {
                Regon = regon
            };

            return await GetLegalEntityAsync(parametryWyszukiwania);
        }

        /// <inheritdoc />
        public async Task<SearchResult> FindByKrsAsync(string krs)
        {
            await loginTask;

            var parametryWyszukiwania = new ParametryWyszukiwania
            {
                Krs = krs
            };

            return await GetLegalEntityAsync(parametryWyszukiwania);
        }

        private async Task<SearchResult> GetLegalEntityAsync(ParametryWyszukiwania parametryWyszukiwania)
        {
            var result = await RunWithSessionScopeAsync(client => client.DaneSzukajPodmiotyAsync(parametryWyszukiwania));

            var xmlResult = result.DaneSzukajPodmiotyResult;

            if (xmlResult.Contains("ErrorCode"))
                return ReturnFailure(xmlResult);

            var serializer = new XmlSerializer(typeof(root));

            var reader = new StringReader(xmlResult);

            var root = (root) serializer.Deserialize(reader);

            var entities = root.dane
                .Select(entity =>
                    new LegalEntity
                    {
                        Regon = entity.Regon,
                        Nip = entity.Nip,
                        Name = entity.Nazwa,
                        Voivodeship = entity.Wojewodztwo,
                        District = entity.Powiat,
                        Commune = entity.Gmina,
                        City = entity.Miejscowosc,
                        PostalCode = entity.KodPocztowy,
                        Post = entity.MiejscowoscPoczty,
                        Street = entity.Ulica,
                        BuildingNumber = string.IsNullOrWhiteSpace(entity.NrNieruchomosci) ? null : entity.NrNieruchomosci,
                        LocalNumber = string.IsNullOrWhiteSpace(entity.NrLokalu) ? null : entity.NrLokalu,
                    }
                );

            return SearchResult.CreateSuccess(entities.ToList());
        }

        private static SearchResult ReturnFailure(string xmlResult)
        {
            var errorSerializer = new XmlSerializer(typeof(Root));

            var reader = new StringReader(xmlResult);

            var errorRoot = (Root) errorSerializer.Deserialize(reader);

            return SearchResult.CreateFailure(errorRoot.Error);
        }

        private async Task<T> RunWithSessionScopeAsync<T>(Func<UslugaBIRzewnPublClient, Task<T>> apiCall)
        {
            Task<T> result;

            using (new OperationContextScope(webServiceClient.InnerChannel))
            {
                var property = new HttpRequestMessageProperty();
                property.Headers.Add("sid", sessionKey);

                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = property;

                result = apiCall(webServiceClient);
            }

            return await result;
        }

        /// <inheritdoc cref="IDisposable" />
        public void Dispose()
        {
            try
            {
                webServiceClient.WylogujAsync(sessionKey).GetAwaiter().GetResult();
            }
            catch
            {
                // ignored
            }
            finally
            {
                ((IDisposable)webServiceClient)?.Dispose();
            }
        }
#if NET5_0_OR_GREATER
        /// <inheritdoc cref="IAsyncDisposable" />
        public async ValueTask DisposeAsync()
        {
            try
            {
                await webServiceClient.WylogujAsync(sessionKey);
            }
            catch
            {
                // ignored
            }
            finally
            {
                ((IDisposable)webServiceClient)?.Dispose();
            }
        }
#endif
    }
}