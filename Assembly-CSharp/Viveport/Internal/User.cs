using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000D43 RID: 3395
	internal class User
	{
		// Token: 0x06005268 RID: 21096 RVA: 0x001A5063 File Offset: 0x001A3263
		static User()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06005269 RID: 21097
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_IsReady")]
		internal static extern int IsReady(StatusCallback IsReadyCallback);

		// Token: 0x0600526A RID: 21098
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_IsReady")]
		internal static extern int IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x0600526B RID: 21099
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_GetUserID")]
		internal static extern int GetUserID(StringBuilder userId, int size);

		// Token: 0x0600526C RID: 21100
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_GetUserID")]
		internal static extern int GetUserID_64(StringBuilder userId, int size);

		// Token: 0x0600526D RID: 21101
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_GetUserName")]
		internal static extern int GetUserName(StringBuilder userName, int size);

		// Token: 0x0600526E RID: 21102
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_GetUserName")]
		internal static extern int GetUserName_64(StringBuilder userName, int size);

		// Token: 0x0600526F RID: 21103
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_GetUserAvatarUrl")]
		internal static extern int GetUserAvatarUrl(StringBuilder userAvatarUrl, int size);

		// Token: 0x06005270 RID: 21104
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUser_GetUserAvatarUrl")]
		internal static extern int GetUserAvatarUrl_64(StringBuilder userAvatarUrl, int size);
	}
}
