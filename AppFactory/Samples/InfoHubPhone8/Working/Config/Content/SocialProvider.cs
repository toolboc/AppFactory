using System;
using BuiltToRoam.BaseClasses;
using BuiltToRoam.Social;

namespace InfoHubPhone8.Config.Content
{
    public class SocialProvider : NotifyBase
    {
        public string ProviderName { get; set; }
        public string TypeName { get; set; }
        public string Icon { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string AdditionalParameter { get; set; }

        private ISocial instance;
        public ISocial Instance
        {
            get
            {
                if (instance == null)
                {
                    var type = Type.GetType(TypeName);
                    if (type == null) return null;
                    instance = Activator.CreateInstance(type) as ISocial;
                    if (instance == null) return null;
                    instance.ApiKey = ApiKey;
                    instance.ApiSecret = ApiSecret;
                    instance.AdditionalParameter = AdditionalParameter;
                    instance.Permissions = SocialPermission.Post | SocialPermission.Profile;
                }
                return instance;
            }
        }

        public void UpdateInstance()
        {
            RaisePropertyChanged(() => Instance);
        }
    }
}