using System;
using System.ComponentModel.Design;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Mapping;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.Data.Metadata;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.ObjectModel.Mapping;
using DocsVision.Platform.ObjectModel.Persistence;
using DocsVision.Platform.SystemCards.ObjectModel.Mapping;
using DocsVision.Platform.SystemCards.ObjectModel.Services;

namespace TestServerExtension
{
    public class DvServer : IDisposable
    {
        public UserSession userSession { get; }
        public ObjectContext objectContext;
        private static string connectionString = "ConnectAddress=http://{SERVERNAME}/DocsVision/StorageServer/StorageServerService.asmx;UserName={USERNAME};Password={PASSWORD}";

        public DvServer()
        {
            SessionManager sessionManager = SessionManager.CreateInstance();
            sessionManager.Connect(connectionString);

            userSession = sessionManager.CreateSession();
            objectContext = CreateObjectContext(userSession);
        }

        internal void Close()
        {
            if (objectContext != null)
            {
                objectContext.Dispose();
                objectContext = null;
            }
            userSession.Close();
        }

        public void Dispose()
        {
            Close();
        }

        private static ObjectContext CreateObjectContext(UserSession userSession)
        {
            ServiceContainer sessionContainer = new ServiceContainer();
            sessionContainer.AddService(typeof(UserSession), userSession);

            ObjectContext objectContext = new ObjectContext(sessionContainer);

            IObjectMapperFactoryRegistry mapperFactoryRegistry = objectContext.GetService<IObjectMapperFactoryRegistry>();
            mapperFactoryRegistry.RegisterFactory(typeof(SystemCardsMapperFactory));
            mapperFactoryRegistry.RegisterFactory(typeof(BackOfficeMapperFactory));

            IServiceFactoryRegistry serviceFactoryRegistry = objectContext.GetService<IServiceFactoryRegistry>();
            serviceFactoryRegistry.RegisterFactory(typeof(SystemCardsServiceFactory));
            serviceFactoryRegistry.RegisterFactory(typeof(BackOfficeServiceFactory));

            objectContext.AddService(DocsVisionObjectFactory.CreatePersistentStore(new SessionProvider(userSession), null));

            IMetadataProvider metadataProvider = DocsVisionObjectFactory.CreateMetadataProvider(userSession);
            objectContext.AddService(DocsVisionObjectFactory.CreateMetadataManager(metadataProvider, userSession));
            objectContext.AddService(metadataProvider);

            return objectContext;
        }

    }
}