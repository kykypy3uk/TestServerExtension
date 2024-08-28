using System;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.StorageServer.Extensibility;

namespace TestServerExtension
{
    public sealed class TestServerExtension : StorageServerExtension
    {
        public TestServerExtension() { }

        [ExtensionMethod]
        public Guid CreateDocument(Guid kindsCardKindId)
        {
            try
            {
                using (DvServer server = new DvServer())
                {
                    IDocumentService documentService = server.objectContext.GetService<IDocumentService>();
                    KindsCardKind kindsCardKind = server.objectContext.GetObject<KindsCardKind>(kindsCardKindId);
                    Document document = documentService.CreateDocument(null, kindsCardKind);
                    server.objectContext.SaveObject(document);

                    return document.GetObjectId();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        [ExtensionMethod]
        public Guid CreateDocumentFromTemplate(Guid templateId)
        {
            try
            {
                using (DvServer server = new DvServer())
                {
                    IDocumentService documentService = server.objectContext.GetService<IDocumentService>();
                    Document document = documentService.CreateDocumentFromTemplate(templateId);
                    server.objectContext.SaveObject(document);

                    return document.GetObjectId();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}