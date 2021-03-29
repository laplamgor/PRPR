using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Services;
using PRPR.BooruViewer.Tasks;
using PRPR.Common.Models.Global;
using PRPR.Common.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace PRPR.BooruViewer.Models.Global
{
    public class YandeSettings : SettingsBase
    {
        public static YandeSettings Current
        {
            get
            {
                if (!Application.Current.Resources.ContainsKey("YandeSettings"))
                {
                    Application.Current.Resources["YandeSettings"] = new YandeSettings();
                }
                return Application.Current.Resources["YandeSettings"] as YandeSettings;
            }
        }

        #region Site info

        public string Host
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "https://konachan.com", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string PasswordHashSalt
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "So-I-Heard-You-Like-Mupkids-?--your-password--", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        #endregion




        #region Login info

        public bool IsLoggedIn
        {
            get
            {
                return UserID != null;
            }
        }

        public string UserName
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), null, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }
        
        public string PasswordHash
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), null, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }
        
        public string UserID
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), null, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(AvatarUri));
                NotifyPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public string AvatarUri
        {
            get
            {
                return $"{YandeClient.HOST}/data/avatars/{UserID}.jpg";
            }
        }

        #endregion


        

        public bool IsRatingFilterUnlocked
        {
            get
            {
                return GetValueOrDefault<bool>(GetCallerName(), false, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }




        private PostFilter _searchPostFilter = null;

        public PostFilter SearchPostFilter
        {
            get
            {
                if (_searchPostFilter == null)
                {
                    var s = GetValueOrDefault<string>(GetCallerName(), null, false);
                    if (s != null)
                    {
                        _searchPostFilter = SerializationService.DeserializeFromString<PostFilter>(s);
                    }
                    else
                    {
                        _searchPostFilter = new PostFilter();
                    }

                    _searchPostFilter.PropertyChanged += SearchPostFilter_PropertyChanged;
                }

                return _searchPostFilter;
            }
            set
            {
                if (_searchPostFilter != null)
                {
                    // Old handler
                    _searchPostFilter.PropertyChanged -= SearchPostFilter_PropertyChanged;
                }

                if (value != null)
                {
                    // Add new handler
                    value.PropertyChanged += SearchPostFilter_PropertyChanged;
                }
                else
                {
                    // WTF
                }



                var s = SerializationService.SerializeToString(value);
                AddOrUpdateValue(GetCallerName(), s, false);
            }
        }

        private void SearchPostFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var s = SerializationService.SerializeToString(_searchPostFilter);
            AddOrUpdateValue(nameof(SearchPostFilter), s, false);
        }




        private PostFilter _wallpaperPostFilter = null;

        public PostFilter WallpaperPostFilter
        {
            get
            {
                if (_wallpaperPostFilter == null)
                {
                    var s = GetValueOrDefault<string>(GetCallerName(), null, false);
                    if (s != null)
                    {
                        _wallpaperPostFilter = SerializationService.DeserializeFromString<PostFilter>(s);
                    }
                    else
                    {
                        _wallpaperPostFilter = new PostFilter();
                    }

                    _wallpaperPostFilter.PropertyChanged += WallpaperPostFilter_PropertyChanged;
                }

                return _wallpaperPostFilter;
            }
            set
            {
                if (_wallpaperPostFilter != null)
                {
                    // Old handler
                    _wallpaperPostFilter.PropertyChanged -= WallpaperPostFilter_PropertyChanged;
                }

                if (value != null)
                {
                    // Add new handler
                    value.PropertyChanged += WallpaperPostFilter_PropertyChanged;
                }
                else
                {
                    // WTF
                }



                var s = SerializationService.SerializeToString(value);
                AddOrUpdateValue(GetCallerName(), s, false);
            }
        }

        private void WallpaperPostFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var s = SerializationService.SerializeToString(_wallpaperPostFilter);
            AddOrUpdateValue(nameof(WallpaperPostFilter), s, false);
        }




        private PostFilter _lockscreenPostFilter = null;

        public PostFilter LockscreenPostFilter
        {
            get
            {
                if (_lockscreenPostFilter == null)
                {
                    var s = GetValueOrDefault<string>(GetCallerName(), null, false);
                    if (s != null)
                    {
                        _lockscreenPostFilter = SerializationService.DeserializeFromString<PostFilter>(s);
                    }
                    else
                    {
                        _lockscreenPostFilter = new PostFilter();
                    }

                    _lockscreenPostFilter.PropertyChanged += LockscreenPostFilter_PropertyChanged;
                }

                return _lockscreenPostFilter;
            }
            set
            {
                if (_lockscreenPostFilter != null)
                {
                    // Old handler
                    _lockscreenPostFilter.PropertyChanged -= LockscreenPostFilter_PropertyChanged;
                }

                if (value != null)
                {
                    // Add new handler
                    value.PropertyChanged += LockscreenPostFilter_PropertyChanged;
                }
                else
                {
                    // WTF
                }



                var s = SerializationService.SerializeToString(value);
                AddOrUpdateValue(GetCallerName(), s, false);
            }
        }

        private void LockscreenPostFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var s = SerializationService.SerializeToString(_lockscreenPostFilter);
            AddOrUpdateValue(nameof(LockscreenPostFilter), s, false);
        }


        


        #region Tile settings


        private PostFilter _tilePostFilter = null;

        public PostFilter TilePostFilter
        {
            get
            {
                if (_tilePostFilter == null)
                {
                    var s = GetValueOrDefault<string>(GetCallerName(), null, false);
                    if (s != null)
                    {
                        _tilePostFilter = SerializationService.DeserializeFromString<PostFilter>(s);
                    }
                    else
                    {
                        _tilePostFilter = new PostFilter();
                    }

                    _tilePostFilter.PropertyChanged += TilePostFilter_PropertyChanged;
                }

                return _tilePostFilter;
            }
            set
            {
                if (_tilePostFilter != null)
                {
                    // Old handler
                    _tilePostFilter.PropertyChanged -= TilePostFilter_PropertyChanged;
                }

                if (value != null)
                {
                    // Add new handler
                    value.PropertyChanged += TilePostFilter_PropertyChanged;
                }
                else
                {
                    // WTF
                }



                var s = SerializationService.SerializeToString(value);
                AddOrUpdateValue(GetCallerName(), s, false);
            }
        }

        private void TilePostFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var s = SerializationService.SerializeToString(_tilePostFilter);
            AddOrUpdateValue(nameof(TilePostFilter), s, false);
        }
        
        public bool TileUpdateTaskEnabled
        {
            get
            {
                return IsTaskRegistered(BackgroundTaskType.Tile);
            }
            set
            {
                if (value == true)
                {
                    BackgroundAccessStatus x = BackgroundAccessStatus.Unspecified;
                    try
                    {
                        try
                        {
                            x = BackgroundExecutionManager.RequestAccessAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {

                        }
                        if (IsTaskRegistered(BackgroundTaskType.Tile))
                        {
                            UnregisterTask(BackgroundTaskType.Tile);
                        }

                        RegisterTask(BackgroundTaskType.Tile);
                    }
                    catch (Exception ex)
                    {
                        new MessageDialog($"{ex.Message}; x={x}; stack={ex.StackTrace}", "Cannot register background task").ShowAsync();
                    }
                }
                else
                {
                    // Unregister the old bg task
                    UnregisterTask(BackgroundTaskType.Tile);
                }
                NotifyPropertyChanged(nameof(TileUpdateTaskEnabled));
            }
        }
        
        public uint TileUpdateTaskTimeSpan
        {
            get
            {
                return GetValueOrDefault<uint>(GetCallerName(), 180, false);
            }
            set
            {
                // Only time between 15min and 1day is allowed
                var valueInRange = Math.Max(15, Math.Min(1440, value));

                // Re-register the task if value changed
                if (valueInRange != TileUpdateTaskTimeSpan && TileUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Tile);
                }

                AddOrUpdateValue(GetCallerName(), valueInRange, false);
            }
        }
        
        public bool TileUpdateTaskOnMeteredNetwork
        {
            get
            {
                return GetValueOrDefault<bool>(GetCallerName(), false, false);
            }
            set
            {
                if (value != TileUpdateTaskOnMeteredNetwork && TileUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Tile);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string TileUpdateTaskAppVersion
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "0.0.0.0", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string TileUpdateTaskSearchKey
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "", false);
            }
            set
            {
                if (value.CompareTo(TileUpdateTaskSearchKey) != 0 && TileUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Tile);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }


        #endregion



        #region Wallpaper settings


        public bool WallpaperUpdateTaskEnabled
        {
            get
            {
                return IsTaskRegistered(BackgroundTaskType.Wallpaper);
            }
            set
            {
                if (value == true)
                {
                    BackgroundAccessStatus x = BackgroundAccessStatus.Unspecified;
                    try
                    {
                        try
                        {
                            x = BackgroundExecutionManager.RequestAccessAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {

                        }
                        if (IsTaskRegistered(BackgroundTaskType.Wallpaper))
                        {
                            UnregisterTask(BackgroundTaskType.Wallpaper);
                        }

                        RegisterTask(BackgroundTaskType.Wallpaper);
                    }
                    catch (Exception ex)
                    {
                        new MessageDialog($"{ex.Message}; x={x}; stack={ex.StackTrace}", "Cannot register background task").ShowAsync();
                    }
                }
                else
                {
                    // Unregister the old bg task
                    UnregisterTask(BackgroundTaskType.Wallpaper);
                }
                NotifyPropertyChanged(nameof(WallpaperUpdateTaskEnabled));
            }
        }

        public uint WallpaperUpdateTaskTimeSpan
        {
            get
            {
                return GetValueOrDefault<uint>(GetCallerName(), 180, false);
            }
            set
            {
                // Only time between 15min and 1day is allowed
                var valueInRange = Math.Max(15, Math.Min(1440, value));

                // Re-register the task if value changed
                if (valueInRange != WallpaperUpdateTaskTimeSpan && WallpaperUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Wallpaper);
                }

                AddOrUpdateValue(GetCallerName(), valueInRange, false);
            }
        }

        public bool WallpaperUpdateTaskOnMeteredNetwork
        {
            get
            {
                return GetValueOrDefault<bool>(GetCallerName(), false, false);
            }
            set
            {
                if (value != WallpaperUpdateTaskOnMeteredNetwork && WallpaperUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Wallpaper);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string WallpaperUpdateTaskAppVersion
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "0.0.0.0", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string WallpaperUpdateTaskSearchKey
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "", false);
            }
            set
            {
                if (value.CompareTo(WallpaperUpdateTaskSearchKey) != 0 && WallpaperUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Wallpaper);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public uint WallpaperUpdateTaskShuffleCount
        {
            get
            {
                return GetValueOrDefault<uint>(GetCallerName(), 10, false);
            }
            set
            {
                // Only 1 to 20 is allowed
                var valueInRange = Math.Max(1, Math.Min(20, value));

                // Re-register the task if value changed
                if (valueInRange != WallpaperUpdateTaskShuffleCount && WallpaperUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Wallpaper);
                }

                AddOrUpdateValue(GetCallerName(), valueInRange, false);
            }
        }

        public string WallpaperUpdateTaskCurrentImageID
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), null, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(WallpaperUpdateTaskCurrentImageFileUri));
            }
        }

        public string WallpaperUpdateTaskCurrentImageFileUri
        {
            get
            {
                return $"ms-appdata:///local/{AnimePersonalization.WALLPAPER_FOLDER_NAME}/{WallpaperUpdateTaskCurrentImageID}.jpg";
            }
        }
        
        public int WallpaperUpdateTaskCropMethodIndex
        {
            get
            {
                return GetValueOrDefault<int>(GetCallerName(), 4, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(WallpaperUpdateTaskCropMethod));
            }
        }

        public CropMethod WallpaperUpdateTaskCropMethod
        {
            get
            {
                if (WallpaperUpdateTaskCropMethodIndex == -1)
                {
                    return CropMethod.None;
                }
                else
                {
                    return (CropMethod)WallpaperUpdateTaskCropMethodIndex;
                }
            }
        }

        public int WallpaperUpdateTaskQuality
        {
            get
            {
                return GetValueOrDefault<int>(GetCallerName(), 0, false);
            }
            set
            {
                // Re-register the task if value changed
                if (value != WallpaperUpdateTaskTimeSpan && WallpaperUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Wallpaper);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }


        #endregion



        #region Lockscreen settings


        public bool LockscreenUpdateTaskEnabled
        {
            get
            {
                return IsTaskRegistered(BackgroundTaskType.Lockscreen);
            }
            set
            {
                if (value == true)
                {
                    BackgroundAccessStatus x = BackgroundAccessStatus.Unspecified;
                    try
                    {
                        try
                        {
                            x = BackgroundExecutionManager.RequestAccessAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {

                        }
                        if (IsTaskRegistered(BackgroundTaskType.Lockscreen))
                        {
                            UnregisterTask(BackgroundTaskType.Lockscreen);
                        }

                        RegisterTask(BackgroundTaskType.Lockscreen);
                    }
                    catch (Exception ex)
                    {
                        new MessageDialog($"{ex.Message}; x={x}; stack={ex.StackTrace}", "Cannot register background task").ShowAsync();
                    }
                }
                else
                {
                    // Unregister the old bg task
                    UnregisterTask(BackgroundTaskType.Lockscreen);
                }
                NotifyPropertyChanged(nameof(LockscreenUpdateTaskEnabled));
            }
        }

        public uint LockscreenUpdateTaskTimeSpan
        {
            get
            {
                return GetValueOrDefault<uint>(GetCallerName(), 180, false);
            }
            set
            {
                // Only time between 15min and 1day is allowed
                var valueInRange = Math.Max(15, Math.Min(1440, value));

                // Re-register the task if value changed
                if (valueInRange != LockscreenUpdateTaskTimeSpan && LockscreenUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Lockscreen);
                }

                AddOrUpdateValue(GetCallerName(), valueInRange, false);
            }
        }

        public bool LockscreenUpdateTaskOnMeteredNetwork
        {
            get
            {
                return GetValueOrDefault<bool>(GetCallerName(), false, false);
            }
            set
            {
                if (value != LockscreenUpdateTaskOnMeteredNetwork && LockscreenUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Lockscreen);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string LockscreenUpdateTaskAppVersion
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "0.0.0.0", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public string LockscreenUpdateTaskSearchKey
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "", false);
            }
            set
            {
                if (value.CompareTo(LockscreenUpdateTaskSearchKey) != 0 && LockscreenUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Lockscreen);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        public uint LockscreenUpdateTaskShuffleCount
        {
            get
            {
                return GetValueOrDefault<uint>(GetCallerName(), 10, false);
            }
            set
            {
                // Only 1 to 20 is allowed
                var valueInRange = Math.Max(1, Math.Min(20, value));

                // Re-register the task if value changed
                if (valueInRange != LockscreenUpdateTaskShuffleCount && LockscreenUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Lockscreen);
                }

                AddOrUpdateValue(GetCallerName(), valueInRange, false);
            }
        }

        public string LockscreenUpdateTaskCurrentImageID
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), null, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(LockscreenUpdateTaskCurrentImageFileUri));
            }
        }

        public string LockscreenUpdateTaskCurrentImageFileUri
        {
            get
            {
                return $"ms-appdata:///local/{AnimePersonalization.LOCKSCREEN_FOLDER_NAME}/{LockscreenUpdateTaskCurrentImageID}.jpg";
            }
        }
        
        public int LockscreenUpdateTaskCropMethodIndex
        {
            get
            {
                return GetValueOrDefault<int>(GetCallerName(), 4, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(LockscreenUpdateTaskCropMethod));
            }
        }

        public CropMethod LockscreenUpdateTaskCropMethod
        {
            get
            {
                if (LockscreenUpdateTaskCropMethodIndex == -1)
                {
                    return CropMethod.None;
                }
                else
                {
                    return (CropMethod)LockscreenUpdateTaskCropMethodIndex;
                }
            }
        }



        public int LockscreenUpdateTaskQuality
        {
            get
            {
                return GetValueOrDefault<int>(GetCallerName(), 0, false);
            }
            set
            {
                // Re-register the task if value changed
                if (value != LockscreenUpdateTaskTimeSpan && LockscreenUpdateTaskEnabled)
                {
                    ReregisterTask(BackgroundTaskType.Lockscreen);
                }

                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        #endregion





        public string CurrentAppVersion
        {
            get
            {
                string appVersion = string.Format("{0}.{1}.{2}.{3}",
                        Package.Current.Id.Version.Major,
                        Package.Current.Id.Version.Minor,
                        Package.Current.Id.Version.Build,
                        Package.Current.Id.Version.Revision);

                return appVersion;
            }
        }








        private bool IsTaskRegistered(BackgroundTaskType type)
        {
            var taskName = GetTaskName(type);

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name.Contains(taskName))
                {
                    return true;
                }
            }
            return false;
        }

        private void UnregisterTask(BackgroundTaskType type)
        {
            //BackgroundExecutionManager.RequestAccessAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                var taskName = GetTaskName(type);
                if (task.Value.Name.Contains(taskName))
                {
                    // Unregister the old bg task
                    task.Value.Unregister(true);
                    Debug.WriteLine($"Unregistered old {taskName}");
                }
            }
        }

        private void RegisterTask(BackgroundTaskType type)
        {
            try
            {
                var builder = new BackgroundTaskBuilder();
                
                builder.Name = GetTaskName(type);
                
#if DEBUG
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
                builder.SetTrigger(new TimeTrigger(GetTaskTimeSpan(type), false));
#else
                builder.SetTrigger(new TimeTrigger(GetTaskTimeSpan(type), false));
#endif


                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                if (!GetTaskOnMeteredNetwork(type))
                {
                    builder.AddCondition(new SystemCondition(SystemConditionType.FreeNetworkAvailable));
                }

                // If the condition changes while the background task is executing then it will be canceled.
                //builder.CancelOnConditionLoss = true;

                var r = builder.Register();

                // Update the version of the task
                SetTaskAppVersion(type, CurrentAppVersion);
                

                Debug.WriteLine($"Registered {builder.Name}");
            }
            catch (Exception ex)
            {
                // TODO: debug
                //new MessageDialog(ex.Message).ShowAsync();
                throw;
            }
        }
        
        private void ReregisterTask(BackgroundTaskType type)
        {
            UnregisterTask(type);
            RegisterTask(type);
        }



        private uint GetTaskTimeSpan(BackgroundTaskType type)
        {
            switch (type)
            {
                case BackgroundTaskType.Lockscreen:
                    return LockscreenUpdateTaskTimeSpan;
                case BackgroundTaskType.Wallpaper:
                    return WallpaperUpdateTaskTimeSpan;
                case BackgroundTaskType.Tile:
                    return TileUpdateTaskTimeSpan;
                default:
                    break;
            }
            return TileUpdateTaskTimeSpan;
        }

        private string GetTaskName(BackgroundTaskType type)
        {
            switch (type)
            {
                case BackgroundTaskType.Lockscreen:
                    return nameof(LockscreenUpdateTask);
                case BackgroundTaskType.Wallpaper:
                    return nameof(WallpaperUpdateTask);
                case BackgroundTaskType.Tile:
                    return nameof(TileUpdateTask);
                default:
                    break;
            }
            return nameof(TileUpdateTask);
        }

        private bool GetTaskOnMeteredNetwork(BackgroundTaskType type)
        {
            switch (type)
            {
                case BackgroundTaskType.Lockscreen:
                    return LockscreenUpdateTaskOnMeteredNetwork;
                case BackgroundTaskType.Wallpaper:
                    return WallpaperUpdateTaskOnMeteredNetwork;
                case BackgroundTaskType.Tile:
                    return TileUpdateTaskOnMeteredNetwork;
                default:
                    break;
            }
            return TileUpdateTaskOnMeteredNetwork;
        }

        private void SetTaskAppVersion(BackgroundTaskType type, string value)
        {
            switch (type)
            {
                case BackgroundTaskType.Lockscreen:
                    LockscreenUpdateTaskAppVersion = value;
                    break;
                case BackgroundTaskType.Wallpaper:
                    WallpaperUpdateTaskAppVersion = value;
                    break;
                case BackgroundTaskType.Tile:
                    TileUpdateTaskAppVersion = value;
                    break;
                default:
                    break;
            }
        }





        #region other settings



        public string DefaultDownloadPath
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), null, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }


        public bool IsDefaultDownloadPathEnabled
        {
            get
            {
                return GetValueOrDefault<bool>(GetCallerName(), false, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }

        #endregion





        public enum BackgroundTaskType
        {
            Lockscreen = 0,
            Wallpaper,
            Tile,
        }

    }

}
