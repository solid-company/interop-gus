using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using SolidCompany.Interop.Gus.Bir;
using SolidCompany.Interop.Gus.Connected_Services;
using SolidCompany.Interop.Gus.Models;
using SolidCompany.Interop.Gus.Models.Generated;
using WcfCoreMtomEncoder;

namespace SolidCompany.Interop.Gus
{
    /// <summary>
    /// Abstraction for GUS BIR API.
    /// </summary>
    public interface IGusBirClient
    {
        /// <summary>
        /// Searches for legal entity.
        /// </summary>
        /// <param name="nip">NIP (Polish TAX ID)</param>
        /// <returns>Return legal entity data.</returns>
        Task<LegalEntity> FindByNipAsync(string nip);
    }

    /// <inheritdoc cref="IGusBirClient" />
    public sealed class GusBirClient : IGusBirClient, IDisposable
#if NET5_0
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
        /// <param name="enironment">GUS environment</param>
        /// <exception cref="ArgumentNullException">Thrown when key or environment is not specified.</exception>
        public GusBirClient(string key, GusEnironment enironment)
        {
            _ = key ?? throw new ArgumentNullException(nameof(key), "Key not specified.");
            _ = enironment ?? throw new ArgumentNullException(nameof(enironment), "Environment not specified.");

            var mtomMessageEncoderBindingElement = new MtomMessageEncoderBindingElement(new TextMessageEncodingBindingElement());
            var httpsBindingElement = new HttpsTransportBindingElement();

            var customBinding = new CustomBinding(mtomMessageEncoderBindingElement, httpsBindingElement);

            webServiceClient = new UslugaBIRzewnPublClient(customBinding, new EndpointAddress(enironment.ServiceUrl));

            loginTask = LogInAsync(key);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">GusBirClient options - <see cref="GusBirOptions"/></param>
        public GusBirClient(IOptions<GusBirOptions> options)
            : this(options.Value.Key, options.Value.Environment)
        {
        }

        private async Task LogInAsync(string key)
        {
            var response = await webServiceClient.ZalogujAsync(key);

            if (response.ZalogujResult?.Length != 20)
                throw new InvalidOperationException();

            sessionKey = response.ZalogujResult;
        }

        /// <inheritdoc />
        public async Task<LegalEntity> FindByNipAsync(string nip)
        {
            await loginTask;

            var pParametryWyszukiwania = new ParametryWyszukiwania
            {
                Nip = nip
            };

            var result = await RunWithSessionScopeAsync(client => client.DaneSzukajPodmiotyAsync(pParametryWyszukiwania));

            var xmlResult = result.DaneSzukajPodmiotyResult;

            var serializer = new XmlSerializer(typeof(root));

            var reader = new StringReader(xmlResult);

            var root = (root)serializer.Deserialize(reader);

            if (root?.dane.Length != 1)
                return default;

            var entity = root.dane[0];

            return new LegalEntity
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
            };
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
#if NET5_0
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