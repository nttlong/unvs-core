using Ntreev.Library.Psd;
namespace unvs.editor.psd {
    /// <summary>
    /// This is the file object of psd file.
    /// </summary>
    public class PsdFileObject {
        private string _path;

        public PsdFileObject(string path) {
                _path= path;
        }
        public void AddLayer(string layerName) {
            
        }
        public PsdDocument CreatePsdDocument() {
            return PsdDocument.Create(_path);
        }
    }
}