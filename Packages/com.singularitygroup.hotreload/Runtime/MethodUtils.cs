#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Reflection;
using SingularityGroup.HotReload.Interop;

namespace SingularityGroup.HotReload {
    static class MethodUtils {
#if ENABLE_MONO
        public static unsafe void DisableVisibilityChecks(MethodBase method) {
            if(IntPtr.Size == sizeof(long)) {
                var ptr = (MonoMethod64*)method.MethodHandle.Value.ToPointer();
                ptr->monoMethodFlags |= MonoMethodFlags.skip_visibility;
            } else {
                var ptr = (MonoMethod32*)method.MethodHandle.Value.ToPointer();
                ptr->monoMethodFlags |= MonoMethodFlags.skip_visibility;
            }
        }

        public static unsafe bool IsMethodInlined(MethodBase method) {
            if(IntPtr.Size == sizeof(long)) {
                var ptr = (MonoMethod64*)method.MethodHandle.Value.ToPointer();
                return (ptr -> monoMethodFlags & MonoMethodFlags.inline_info) == MonoMethodFlags.inline_info;
            } else {
                var ptr = (MonoMethod32*)method.MethodHandle.Value.ToPointer();
                return (ptr -> monoMethodFlags & MonoMethodFlags.inline_info) == MonoMethodFlags.inline_info;
            }
        }
#else
        public static void DisableVisibilityChecks(MethodBase method) { }
        public static bool IsMethodInlined(MethodBase method) {
             return false; 
        }
#endif
    }
}
#endif
