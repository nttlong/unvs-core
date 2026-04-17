using Ntreev.Library.Psd;
namespace unvs.editor.psd {
    public static class EditorPsdUtils {
        public static PsdFileObject CreatePsdFileObject(string path) {
            return new PsdFileObject(path);
        }
    }
}
