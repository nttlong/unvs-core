using Cysharp.Threading.Tasks;
using Ntreev.Library.Psd;

namespace unvs.editor.psd
{
    public static class EditorPsdUtils
    {
        public static PsdFileObject CreatePsdFileObject(string path)
        {
            using (PsdDocument document = PsdDocument.Create(path))
            {
              
                // Add layers or manipulate document structure
            }
            return new PsdFileObject(path);
        }


        // Create a new PSD document

    }
}
