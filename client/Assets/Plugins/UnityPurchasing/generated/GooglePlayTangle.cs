#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("s/Tlpwj+Qn5Gq5irMi71QuSLMqoomhk6KBUeETKeUJ7vFRkZGR0YG6b+qE+mg6bWOdv6dro7d5UAuoSkgBxs7M+hWFYfqe7M2BHElaQYLuXsoDHvYzi53YD7TD71rASqB3/8Jnt0VUX5NvhOuZmrNSql9s7/MGZXblmQ9U+fzrxBmvRGYiJBWwtaKmRCElhcQDm+DcpAKMByTciArkTnTpoZFxgomhkSGpoZGRi4pvFKVQxx4bY9ccHfNfcFCrRN03jV+2yDYnKPjiphnMjrE3OrbpCfjzFUACUXqCo/A1D6s9LExRCKdOV/G3PCzuhO8Z6JH/XBgijBQO1lgC3XkzpUd59+3sr+XCj/czfbPGl4O1tXFi4uXxEBH4nmyq0g6RobGRgZ");
        private static int[] order = new int[] { 8,8,5,13,10,5,13,9,8,9,10,11,12,13,14 };
        private static int key = 24;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
