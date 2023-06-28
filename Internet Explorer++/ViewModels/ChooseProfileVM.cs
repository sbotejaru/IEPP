using IEPP.Controls;
using IEPP.Enums;
using IEPP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IEPP.ViewModels
{
    public class ChooseProfileVM : INotifyPropertyChanged
    {
        #region PropertyChanged declaration

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private UserContainer selectedUser;
        private Visibility addNewVisibility;

        private string workingDir;
        public string WorkingDir
        {
            get { return workingDir; }
            set { workingDir = value; LoadUsers(); }
        }

        private Visibility vis;
        public Visibility Vis
        {
            get { return vis; }
            set
            {
                vis = value;

                if (vis == Visibility.Visible)
                    ChooseProfileVisibility = Visibility.Visible;
                else
                    ChooseProfileVisibility = Visibility.Collapsed;

                NotifyPropertyChanged("Vis");
            }
        }

        private void CreateNewUserDirectory(string username, Uri avatarSrc)
        {
            var newUserDir = WorkingDir + "/" + username;
            bool exists = Directory.Exists(newUserDir);

            if (!exists)
            {
                Directory.CreateDirectory(newUserDir);
                var userAvatarPath = newUserDir + "/avatar.jpg";
                SaveAvatar(userAvatarPath, avatarSrc);
            }
        }

        private void EditSelectedUserDirectory(string newUsername)
        {
            var userDir = WorkingDir + "/" + SelectedUser.Username;
            var newUserDir = WorkingDir + "/" + newUsername;

            Directory.Move(userDir, newUserDir);

            SelectedUser.Username = newUsername;
        }

        private void EditSelectedUserDirectory(Uri newAvatarSrc)
        {
            var avatarDir = WorkingDir + "/" + SelectedUser.Username + "/avatar.jpg";
            SaveAvatar(avatarDir, newAvatarSrc);

            BitmapDecoder decoder = BitmapDecoder.Create(new Uri(avatarDir), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
            BitmapSource avatarSource = decoder.Frames[0];

            SelectedUser.ProfilePic = ConvertBitmapSourceToBitmapImage(avatarSource);
        }

        private void EditSelectedUserDirectory(string newUsername, Uri newAvatarSrc)
        {
            EditSelectedUserDirectory(newAvatarSrc);
            EditSelectedUserDirectory(newUsername);
        }

        private void DeleteSelectedUser()
        {
            var userPath = WorkingDir + "/" + SelectedUser.Username;

            foreach (var user in UserList)
            {
                if (user.Username == SelectedUser.Username)
                {
                    UserList.Remove(user);
                    break;
                }
            }

            Directory.Delete(userPath, true);
            Reset();
            SetVisToChooseList();
        }

        private void SaveAvatar(string avatarPath, Uri avatarSrc)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(avatarSrc));
            using (FileStream stream = new FileStream(avatarPath, FileMode.Create))
                encoder.Save(stream);
        }

        public void LoadUsers()
        {
            string[] users = Directory.GetDirectories(WorkingDir);
            if (users.Length == 0)
                return;

            if (users.Length == 5)
                AddNewVisibility = Visibility.Collapsed;

            foreach (var user in users)
            {
                var username = Path.GetFileName(user);
                AddToUserList(username);
            }
        }

        private bool InUserList(string username)
        {
            foreach (var user in UserList)
            {
                if (user.Username == username)
                    return true;
            }

            return false;
        }

        private void AddToUserList(string username)
        {
            var userPath = WorkingDir + "/" + username;
            var avatarPath = new Uri(userPath + "/avatar.jpg");

            BitmapDecoder decoder = BitmapDecoder.Create(avatarPath, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
            BitmapSource avatarSource = decoder.Frames[0];

            UserList.Add(new UserContainer() { Username = username, ProfilePic = ConvertBitmapSourceToBitmapImage(avatarSource) });
        }

        public static BitmapImage ConvertBitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            // before encoding/decoding, check if bitmapSource is already a BitmapImage
            if (!(bitmapSource is BitmapImage bitmapImage))
            {
                bitmapImage = new BitmapImage();

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    memoryStream.Position = 0;

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }
            }

            return bitmapImage;
        }

        private void InitAttrbiuteValues()
        {
            SelectedGender = -1;
            SelectedEthnicity = -1;
            SelectedHairColor = -1;
            SelectedHairStyle = -1;
            SelectedAge = 0;
            HasBangs = false;
            HasBeard = false;
            HasGlasses = false;
            IsBald = false;
        }

        private void SetVisToChooseList()
        {
            ChooseProfileVisibility = Visibility.Visible;
            NewProfileVisibility = Visibility.Collapsed;
        }

        private void HideErrors()
        {
            NewProfileError = Visibility.Hidden;
        }

        private void Reset()
        {
            HideErrors();
            InitAttrbiuteValues();
            NewUsername = "";
            AvatarImage = new BitmapImage(new Uri("\\Icons\\avatar_placeholder.png", UriKind.Relative));
            AvatarSaved = false;
            AvatarChanged = false;
            FacialHairOpacity = 1;
            HairAttributesOpacity = 1;
            LockMode = false;
            IsLockedOpacity = 1;
            IsBaldEnabled = true;
            IsBaldOpacity = 1;

            if (File.Exists(NewAvatarPath))
                File.Delete(NewAvatarPath);
        }

        // Init function that sets all values to null / resets everything
        private void Init()
        {
            LoadingScreenVisibility = Visibility.Collapsed;
            ChooseProfileVisibility = Visibility.Collapsed;
            NewProfileVisibility = Visibility.Collapsed;
            CreateAvatarVisibility = Visibility.Collapsed;
            UserList = new ObservableCollection<UserContainer>();
            NewUsername = "";
            NewProfileErrorMessage = "A profile with that name already exists!";
            FacialHairOpacity = 1;
            HairAttributesOpacity = 1;
            IsLockedOpacity = 1;
            AvatarImage = new BitmapImage(new Uri("\\Icons\\avatar_placeholder.png", UriKind.Relative));
            LoadingText = "";
            AvatarChanged = false;
            AvatarSaved = false;
            DefaultAvatarPath = "pack://application:,,,/Internet Explorer++;component/Icons/coffee.jpg";
            NewAvatarPath = AppDomain.CurrentDomain.BaseDirectory + "/avatar_temp.jpg";
            EditButtonContent = "Edit";
            EditVisibility = Visibility.Collapsed;
            EditMode = false;
            DeleteButtonVisibility = Visibility.Collapsed;
            LockMode = false;

            HideErrors();
            InitAttrbiuteValues();
        }

        private void ToEditMode()
        {
            EditMode = true;
            EditVisibility = Visibility.Visible;
            EditButtonContent = "Done";
            AddNewVisibility = Visibility.Collapsed;
            DeleteButtonVisibility = Visibility.Visible;
        }

        private void ToNormalMode()
        {
            EditMode = false;
            EditVisibility = Visibility.Collapsed;
            EditButtonContent = "Edit";
            DeleteButtonVisibility = Visibility.Collapsed;

            if (UserList.Count < 5)
                AddNewVisibility = Visibility.Visible;
        }

        private void RandomizeAttributes()
        {
            Random random = new Random();

            int RandomGender = random.Next(0, 2);
            int RandomEthnicity = random.Next(0, 6);
            int RandomAge = random.Next(1, 60);
            int RandomHairColor = random.Next(0, 4);
            int RandomHairStyle = random.Next(0, 3);
            int RandomMoustache = random.Next(0, 2);
            int RandomBeard = random.Next(0, 2);
            int RandomAccessory = random.Next(0, 2);
            int RandomBald = random.Next(0, 2);
            int RandomBangs = random.Next(0, 2);

            SelectedGender = RandomGender;

            if (!IsFemale)
            {
                HasBeard = true ? RandomBeard == 1 : RandomBeard == 0;
                HasMoustache = true ? RandomMoustache == 1 : RandomMoustache == 0;
                IsBald = true ? RandomBald == 1 : RandomBald == 0;
            }

            if (!IsBald)
            {
                HasBangs = true ? RandomBangs == 1 : RandomBangs == 0;
                SelectedHairColor = RandomHairColor;
                SelectedHairStyle = RandomHairStyle;
            }

            SelectedEthnicity = RandomEthnicity;
            SelectedAge = RandomAge;

            HasGlasses = true ? RandomAccessory == 1 : RandomAccessory == 0;

        }

        private void EditSelectedProfile()
        {
            NewUsername = SelectedUser.Username;
            var userPath = WorkingDir + "/" + SelectedUser.Username;
            var avatarPath = userPath + "/avatar.jpg";

            ChooseProfileVisibility = Visibility.Collapsed;
            NewProfileVisibility = Visibility.Visible;

            if (File.Exists(avatarPath))
            {
                BitmapDecoder decoder = BitmapDecoder.Create(new Uri(avatarPath), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
                AvatarImage = decoder.Frames[0];
            }
        }

        // visibilities binds
        #region Visibilities

        private Visibility newProfileError;
        public Visibility NewProfileError
        {
            get { return newProfileError; }
            set { newProfileError = value; NotifyPropertyChanged("NewProfileError"); }
        }

        public Visibility AddNewVisibility
        {
            get { return addNewVisibility; }
            set { addNewVisibility = value; NotifyPropertyChanged("AddNewVisibility"); }
        }

        private Visibility chooseVisibility;
        public Visibility ChooseProfileVisibility
        {
            get { return chooseVisibility; }
            set { chooseVisibility = value; NotifyPropertyChanged("ChooseProfileVisibility"); }
        }

        private Visibility newProfileVisibility;
        public Visibility NewProfileVisibility
        {
            get { return newProfileVisibility; }
            set { newProfileVisibility = value; NotifyPropertyChanged("NewProfileVisibility"); }
        }

        private Visibility createAvatarVisibility;
        public Visibility CreateAvatarVisibility
        {
            get { return createAvatarVisibility; }
            set { createAvatarVisibility = value; NotifyPropertyChanged("CreateAvatarVisibility"); }
        }

        #endregion

        public UserContainer SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                NotifyPropertyChanged("SelectedUser");

                if (!EditMode)
                {
                    if (selectedUser.Username != "")
                        CurrentUserPath = WorkingDir + "/" + selectedUser.Username;
                    else
                        CurrentUserPath = "";
                }
                else if (selectedUser != null)
                {
                    EditSelectedProfile();
                }
            }
        }

        private string currentUserPath;
        public string CurrentUserPath
        {
            get { return currentUserPath; }
            set { currentUserPath = value; }
        }

        private string newUsername;
        public string NewUsername
        {
            get { return newUsername; }
            set { newUsername = value; NotifyPropertyChanged("NewUsername"); }
        }

        private string newProfileErrorMessage;
        public string NewProfileErrorMessage
        {
            get { return newProfileErrorMessage; }
            set { newProfileErrorMessage = value; NotifyPropertyChanged("NewProfileErrorMessage"); }
        }

        private string editButtonContent;
        public string EditButtonContent
        {
            get { return editButtonContent; }
            set { editButtonContent = value; NotifyPropertyChanged("EditButtonContent"); }
        }

        private Visibility editVisibility;
        public Visibility EditVisibility
        {
            get { return editVisibility; }
            set
            {
                editVisibility = value;

                if (editVisibility == Visibility.Visible)
                    WhosBrowsingVisibility = Visibility.Hidden;
                else
                    WhosBrowsingVisibility = Visibility.Visible;

                NotifyPropertyChanged("EditVisibility");
            }
        }

        private Visibility deleteBtnVis;
        public Visibility DeleteButtonVisibility
        {
            get { return deleteBtnVis; }
            set { deleteBtnVis = value; NotifyPropertyChanged("DeleteButtonVisibility"); }
        }

        public bool EditMode { get; set; }

        private bool lockMode;
        public bool LockMode
        {
            get { return lockMode; }
            set
            {
                lockMode = value;
                if (lockMode)
                {
                    IsLockedOpacity = 0.5;
                    IsBaldOpacity = 0.5;
                    IsBaldEnabled = false;
                }
                else
                {
                    IsLockedOpacity = 1;
                    if (!IsFemale)
                    {
                        IsBaldOpacity = 1;
                        IsBaldEnabled = true;
                    }
                }

                NotifyPropertyChanged("LockMode");
            }
        }

        private Visibility whosBrowsingVis;
        public Visibility WhosBrowsingVisibility
        {
            get { return whosBrowsingVis; }
            set { whosBrowsingVis = value; NotifyPropertyChanged("WhosBrowsingVisibility"); }
        }

        private ImageSource generatedImage;
        public ImageSource AvatarImage
        {
            get { return generatedImage; }
            set { generatedImage = value; NotifyPropertyChanged("AvatarImage"); AvatarChanged = true; }
        }

        private bool AvatarChanged { get; set; }
        private bool AvatarSaved { get; set; }

        private Visibility loadingScreenVisibility;
        public Visibility LoadingScreenVisibility
        {
            get { return loadingScreenVisibility; }
            set { loadingScreenVisibility = value; NotifyPropertyChanged("LoadingScreenVisibility"); }
        }

        private string DefaultAvatarPath { get; set; }
        private string NewAvatarPath { get; set; }

        // avatar attributes binds
        #region avatar attributes

        private int selectedGender;
        public int SelectedGender
        {
            get { return selectedGender; }
            set
            {
                selectedGender = value;
                IsFemale = true ? selectedGender == 1 : selectedGender == 0;
                NotifyPropertyChanged("SelectedGender");
            }
        }

        private double facialHairOpacity;
        public double FacialHairOpacity
        {
            get { return facialHairOpacity; }
            set { facialHairOpacity = value; NotifyPropertyChanged("FacialHairOpacity"); }
        }

        private double hairAttributesOpacity;
        public double HairAttributesOpacity
        {
            get { return hairAttributesOpacity; }
            set { hairAttributesOpacity = value; NotifyPropertyChanged("HairAttributesOpacity"); }
        }

        private double isLockedOpacity;
        public double IsLockedOpacity
        {
            get { return isLockedOpacity; }
            set { isLockedOpacity = value; NotifyPropertyChanged("IsLockedOpacity"); }
        }

        private int selectedEthnicity;
        public int SelectedEthnicity
        {
            get { return selectedEthnicity; }
            set
            {
                selectedEthnicity = value;
                NotifyPropertyChanged("SelectedEthnicity");
            }
        }

        private int selectedHairColor;
        public int SelectedHairColor
        {
            get { return selectedHairColor; }
            set
            {
                selectedHairColor = value;

                if (LockMode)
                    HairColorChanged = true;

                NotifyPropertyChanged("SelectedHairColor");
            }
        }

        private int selectedHairStyle;
        public int SelectedHairStyle
        {
            get { return selectedHairStyle; }
            set
            {
                selectedHairStyle = value;

                if (LockMode)
                    HairStyleChanged = true;

                NotifyPropertyChanged("SelectedHairStyle");
            }
        }

        private bool isBaldEnabled = true;
        public bool IsBaldEnabled
        {
            get { return isBaldEnabled; }
            set { isBaldEnabled = value; NotifyPropertyChanged("IsBaldEnabled"); }
        }

        private double isBaldOpacity = 1;
        public double IsBaldOpacity
        {
            get { return isBaldOpacity; }
            set { isBaldOpacity = value; NotifyPropertyChanged("IsBaldOpacity"); }
        }

        private int selectedAge;
        public int SelectedAge
        {
            get { return selectedAge; }
            set
            {
                selectedAge = value;

                if (LockMode)
                    AgeChanged = true;

                NotifyPropertyChanged("SelectedAge");
            }
        }

        private bool isBald;
        public bool IsBald
        {
            get { return isBald; }
            set
            {
                isBald = value;

                if (isBald)
                {
                    SelectedHairColor = -1;
                    SelectedHairStyle = -1;
                    HasBangs = false;
                    HairAttributesOpacity = 0.5;
                }
                else
                {
                    HairAttributesOpacity = 1;
                }

                NotifyPropertyChanged("IsBald");
            }
        }

        private bool hasBangs;
        public bool HasBangs
        {
            get { return hasBangs; }
            set
            {
                hasBangs = value;

                if (LockMode)
                    HasBangsChanged = true;

                NotifyPropertyChanged("HasBangs");
            }
        }

        private bool hasGlasses;
        public bool HasGlasses
        {
            get { return hasGlasses; }
            set
            {
                hasGlasses = value;

                if (LockMode)
                    HasGlassesChanged = true;

                NotifyPropertyChanged("HasGlasses");
            }
        }

        private bool hasBeard;
        public bool HasBeard
        {
            get { return hasBeard; }
            set
            {
                hasBeard = value;

                if (LockMode)
                    HasBeardChanged = true;

                NotifyPropertyChanged("HasBeard");
            }
        }

        private bool hasMoustache;
        public bool HasMoustache
        {
            get { return hasMoustache; }
            set { hasMoustache = value; NotifyPropertyChanged("HasMoustache"); }
        }

        private bool isFemale;
        public bool IsFemale
        {
            get { return isFemale; }
            set
            {
                isFemale = value;

                if (isFemale)
                {
                    HasBeard = false;
                    HasMoustache = false;
                    IsBald = false;
                    IsBaldOpacity = 0.5;
                    IsBaldEnabled = false;
                    FacialHairOpacity = 0.5;
                }
                else
                {
                    FacialHairOpacity = 1;
                    IsBaldOpacity = 1;
                    IsBaldEnabled = true;
                }

                NotifyPropertyChanged("IsFemale");
            }
        }

        public bool AgeChanged { get; set; } = false;
        public bool HasBangsChanged { get; set; } = false;
        public bool HairStyleChanged { get; set; } = false;
        public bool HairColorChanged { get; set; } = false;
        public bool HasBeardChanged { get; set; } = false;
        public bool HasGlassesChanged { get; set; } = false;

        #endregion

        private List<double> FeatureList { get; set; }
        private void CreateFeaturesFile()
        {
            var random = new Random();

            if (LockMode && AvatarChanged)
            {
                if (HasBangsChanged)
                {
                    FeatureList[9] = HasBangs ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01;
                    HasBeardChanged = false;
                }
                if (HasGlassesChanged)
                {
                    FeatureList[13] = HasGlasses ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01;
                    HasGlassesChanged = false;
                }
                if (HasBeardChanged)
                {
                    FeatureList[16] = HasBeard ? random.Next(1, 30) * 0.01 : random.Next(70, 101) * 0.01;
                    HasBeardChanged = false;
                }
                if (AgeChanged)
                {
                    FeatureList[19] = (100 - SelectedAge) * 0.01;
                    AgeChanged = false;
                }
                if (HairColorChanged)
                {
                    for (int index = 10; index < 15; ++index)
                        if (index != 13)
                            FeatureList[index] = random.Next(0, 31) * 0.01;

                    if (SelectedHairColor != (int)HairColor.Gray)
                    {
                        if (SelectedHairColor != -1)
                            FeatureList[SelectedHairColor + 10] = random.Next(70, 101) * 0.01;
                    }
                    else
                    {
                        if (SelectedAge < 40)
                        {
                            FeatureList[14] = random.Next(20, 51) * 0.01;
                            FeatureList[11] = random.Next(50, 81) * 0.01;
                        }
                        else
                        {
                            FeatureList[14] = random.Next(45, 71) * 0.01;
                            FeatureList[11] = FeatureList[14];
                        }
                    }

                    HairColorChanged = false;
                }
                if (HairStyleChanged)
                {
                    switch (SelectedHairStyle)
                    {
                        case (int)HairLength.Short:
                            if (IsFemale)
                            {
                                FeatureList[17] = random.Next(20, 41) * 0.01;
                                FeatureList[18] = (random.Next(20, 46) * 0.01);
                            }
                            else
                            {
                                if (IsBald)
                                {
                                    FeatureList[17] = (0);
                                    FeatureList[18] = (0);
                                    break;
                                }

                                FeatureList[17] = (random.Next(0, 21) * 0.01);
                                FeatureList[18] = (random.Next(0, 21) * 0.01);
                                if (HasBangs)
                                    FeatureList[9] = random.Next(41, 61) * 0.01;
                            }
                            break;
                        case (int)HairLength.Medium:
                            if (IsFemale)
                            {
                                FeatureList[17] = (random.Next(41, 66) * 0.01);
                                FeatureList[18] = (random.Next(45, 71) * 0.01);
                            }
                            else
                            {
                                if (IsBald)
                                {
                                    FeatureList[17] = (0);
                                    FeatureList[18] = (0);
                                    break;
                                }

                                FeatureList[17] = (random.Next(20, 41) * 0.01);
                                FeatureList[18] = (random.Next(20, 41) * 0.01);

                                if (HasBangs)
                                    FeatureList[9] = random.Next(60, 81) * 0.01;
                            }
                            break;
                        case (int)HairLength.Long:
                            if (IsFemale)
                            {
                                FeatureList[17] = (random.Next(41, 66) * 0.01);
                                FeatureList[18] = (random.Next(71, 96) * 0.01);
                            }
                            else
                            {
                                if (IsBald)
                                {
                                    FeatureList[17] = (0);
                                    FeatureList[18] = (0);
                                    break;
                                }

                                FeatureList[17] = (random.Next(20, 41) * 0.01);
                                FeatureList[18] = (random.Next(41, 61) * 0.01);
                                if (HasBangs)
                                    FeatureList[9] = random.Next(81, 101) * 0.01;
                            }
                            break;
                        default:
                            if (IsFemale)
                            {
                                FeatureList[17] = (random.Next(50, 76) * 0.01);
                                FeatureList[18] = (random.Next(50, 76) * 0.01);
                            }
                            else
                            {
                                if (IsBald)
                                {
                                    FeatureList[17] = (0);
                                    FeatureList[18] = (0);
                                    break;
                                }

                                FeatureList[17] = (random.Next(20, 41) * 0.01);
                                FeatureList[18] = (random.Next(20, 41) * 0.01);
                            }
                            break;
                    }

                    HairStyleChanged = false;
                }
            }
            else
            {
                FeatureList = new List<double>();

                double female = IsFemale ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01;

                FeatureList.Add(female);
                FeatureList.Add(1 - female);

                for (int index = 0; index < 6; ++index)
                    FeatureList.Add(random.Next(1, 21) * 0.01);

                if (SelectedEthnicity != -1)
                    FeatureList[SelectedEthnicity + 2] = random.Next(75, 101) * 0.01;

                FeatureList.Add(IsBald ? 1 : random.Next(1, 30) * 0.01);

                if (!IsBald)
                    FeatureList.Add(HasBangs ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01);
                else
                    FeatureList.Add(random.Next(0, 21) * 0.01);

                for (int index = 0; index < 5; ++index)
                    FeatureList.Add(random.Next(1, 30) * 0.01);


                FeatureList[13] = HasGlasses ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01;

                if (SelectedHairColor != (int)HairColor.Gray)
                {
                    if (SelectedHairColor != -1)
                        FeatureList[SelectedHairColor + 10] = random.Next(70, 101) * 0.01;
                }
                else
                {
                    if (SelectedAge < 40)
                    {
                        FeatureList[14] = random.Next(20, 51) * 0.01;
                        FeatureList[11] = random.Next(50, 81) * 0.01;
                    }
                    else
                    {
                        FeatureList[14] = random.Next(45, 71) * 0.01;
                        FeatureList[11] = FeatureList[14];
                    }
                }

                if (SelectedEthnicity == (int)Ethnicity.AfroAmerican)
                    FeatureList.Add(random.Next(31, 76) * 0.01);
                else
                {
                    if (SelectedHairColor == (int)HairColor.Gray)
                        FeatureList.Add(random.Next(0, 36) * 0.01);
                    else
                        FeatureList.Add(random.Next(0, 51) * 0.01);

                }

                FeatureList.Add(HasBeard ? random.Next(1, 30) * 0.01 : random.Next(70, 101) * 0.01);

                switch (SelectedHairStyle)
                {
                    case (int)HairLength.Short:
                        if (IsFemale)
                        {
                            FeatureList.Add(random.Next(20, 41) * 0.01);
                            FeatureList.Add(random.Next(20, 46) * 0.01);
                        }
                        else
                        {
                            if (IsBald)
                            {
                                FeatureList.Add(0);
                                FeatureList.Add(0);
                                break;
                            }

                            FeatureList.Add(random.Next(0, 21) * 0.01);
                            FeatureList.Add(random.Next(0, 21) * 0.01);
                            if (HasBangs)
                                FeatureList[9] = random.Next(35, 51) * 0.01;
                        }
                        break;
                    case (int)HairLength.Medium:
                        if (IsFemale)
                        {
                            FeatureList.Add(random.Next(41, 66) * 0.01);
                            FeatureList.Add(random.Next(45, 71) * 0.01);
                        }
                        else
                        {
                            if (IsBald)
                            {
                                FeatureList.Add(0);
                                FeatureList.Add(0);
                                break;
                            }

                            FeatureList.Add(random.Next(20, 41) * 0.01);
                            FeatureList.Add(random.Next(20, 41) * 0.01);
                            if (HasBangs)
                                FeatureList[9] = random.Next(51, 71) * 0.01;
                        }
                        break;
                    case (int)HairLength.Long:
                        if (IsFemale)
                        {
                            FeatureList.Add(random.Next(41, 66) * 0.01);
                            FeatureList.Add(random.Next(71, 96) * 0.01);
                        }
                        else
                        {
                            if (IsBald)
                            {
                                FeatureList.Add(0);
                                FeatureList.Add(0);
                                break;
                            }

                            FeatureList.Add(random.Next(20, 41) * 0.01);
                            FeatureList.Add(random.Next(41, 61) * 0.01);
                            if (HasBangs)
                                FeatureList[9] = random.Next(71, 91) * 0.01;
                        }
                        break;
                    default:
                        if (IsFemale)
                        {
                            FeatureList.Add(random.Next(50, 76) * 0.01);
                            FeatureList.Add(random.Next(50, 76) * 0.01);
                        }
                        else
                        {
                            if (IsBald)
                            {
                                FeatureList.Add(0);
                                FeatureList.Add(0);
                                break;
                            }

                            FeatureList.Add(random.Next(20, 41) * 0.01);
                            FeatureList.Add(random.Next(20, 41) * 0.01);
                        }
                        break;
                }

                FeatureList.Add((100 - SelectedAge) * 0.01);
            }


            File.WriteAllLines("feature_list.txt", FeatureList.Select(x => x.ToString()));
        }


        private string loadingtext;
        public string LoadingText
        {
            get { return loadingtext; }
            set { loadingtext = value; NotifyPropertyChanged("LoadingText"); }
        }

        private int ProcessStep { get; set; }

        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            switch (ProcessStep)
            {
                case 0:
                    LoadingText = "Loading image generator...";
                    break;
                case 1:
                    LoadingText = "Generating image...";
                    break;
                case 2:
                    LoadingText = "Animating your avatar...";
                    break;
                case 3:
                    LoadingText = "Loading your avatar...";
                    break;
                default:
                    break;
            }

            Console.WriteLine(e.Data);
            ++ProcessStep;
        }

        private void BgWorker_StartGenerationProcess(object o, DoWorkEventArgs e)
        {
            LoadingText = "Starting generation process...";
            ProcessStep = 0;

            using (Process pProcess = new Process())
            {
                pProcess.StartInfo.FileName = "generate_avatar\\generate_avatar.exe";
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.Start();
                pProcess.OutputDataReceived += ProcessOutputDataReceived;
                pProcess.BeginOutputReadLine();
                pProcess.WaitForExit();
            }
        }

        private void BgWorker_GenerationCompleted(object o, RunWorkerCompletedEventArgs e)
        {
            LoadingText = "Loading your avatar...";

            if (File.Exists("feature_list.txt"))
                File.Delete("feature_list.txt");

            string loc = AppDomain.CurrentDomain.BaseDirectory + "/avatar_temp.jpg";

            if (File.Exists(loc))
            {
                BitmapDecoder decoder = BitmapDecoder.Create(new Uri(loc), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);

                AvatarImage = decoder.Frames[0];
            }

            LoadingScreenVisibility = Visibility.Collapsed;
            LoadingText = "";
        }

        public ObservableCollection<UserContainer> UserList { get; set; }
        public RelayCommand AddNewProfileCommand { get; set; }
        public RelayCommand GuestCommand { get; set; }
        public RelayCommand SaveProfileCommand { get; set; }
        public RelayCommand CreateAvatarCommand { get; set; }
        public RelayCommand SaveAvatarCommand { get; set; }
        public RelayCommand GenerateAvatarCommand { get; set; }
        public RelayCommand RandomizeAvatarCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand DeleteProfileCommand { get; set; }
        public RelayCommand<int> GoBackCommand { get; set; }

        public ChooseProfileVM()
        {
            Init();

            AddNewProfileCommand = new RelayCommand(o =>
            {
                ChooseProfileVisibility = Visibility.Collapsed;
                NewProfileVisibility = Visibility.Visible;
            });

            SaveProfileCommand = new RelayCommand(o =>
            {
                if (NewUsername != "")
                {
                    if (NewUsername.Length > 16)
                    {
                        NewProfileErrorMessage = "Username must be shorter than 16 characters!";
                        NewProfileError = Visibility.Visible;
                        return;
                    }

                    if (EditMode)
                    {
                        if (AvatarChanged && AvatarSaved && NewUsername != SelectedUser.Username)
                            EditSelectedUserDirectory(NewUsername, new Uri(NewAvatarPath));
                        else if (NewUsername != SelectedUser.Username)
                            EditSelectedUserDirectory(NewUsername);
                        else if (AvatarChanged && AvatarSaved)
                            EditSelectedUserDirectory(new Uri(NewAvatarPath));

                        SelectedUser = null;

                        Reset();
                        SetVisToChooseList();
                    }
                    else
                    {
                        bool exists = InUserList(NewUsername);

                        if (!exists)
                        {

                            if (AvatarChanged && AvatarSaved)
                                CreateNewUserDirectory(NewUsername, new Uri(NewAvatarPath));
                            else
                                CreateNewUserDirectory(NewUsername, new Uri(DefaultAvatarPath));

                            AddToUserList(NewUsername);

                            if (UserList.Count == 5)
                                AddNewVisibility = Visibility.Collapsed;


                            Reset();
                            SetVisToChooseList();
                        }
                        else
                        {
                            NewProfileErrorMessage = "A profile with that name already exists!";
                            NewProfileError = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    SetVisToChooseList();
                }
            });

            CreateAvatarCommand = new RelayCommand(o =>
            {
                NewProfileVisibility = Visibility.Collapsed;
                CreateAvatarVisibility = Visibility.Visible;
            });

            SaveAvatarCommand = new RelayCommand(o =>
            {
                if (AvatarChanged)
                {
                    AvatarSaved = true;
                    CreateAvatarVisibility = Visibility.Collapsed;
                    NewProfileVisibility = Visibility.Visible;
                }
            });

            GoBackCommand = new RelayCommand<int>(index =>
            {
                switch (index)
                {
                    case 0:
                        Reset();
                        SetVisToChooseList();
                        break;
                    case 1:
                        CreateAvatarVisibility = Visibility.Collapsed;
                        NewProfileVisibility = Visibility.Visible;
                        break;
                }
            });

            GenerateAvatarCommand = new RelayCommand(o =>
            {
                LoadingScreenVisibility = Visibility.Visible;
                LoadingText = "Processing Data...";
                CreateFeaturesFile();

                var bgWorker = new BackgroundWorker();
                bgWorker.DoWork += BgWorker_StartGenerationProcess;
                bgWorker.RunWorkerCompleted += BgWorker_GenerationCompleted;
                bgWorker.RunWorkerAsync();
            });

            RandomizeAvatarCommand = new RelayCommand(o =>
            {
                RandomizeAttributes();
            });

            EditCommand = new RelayCommand(o =>
            {
                if (!EditMode)
                    ToEditMode();
                else
                    ToNormalMode();
            });

            DeleteProfileCommand = new RelayCommand(o =>
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete the user \"" + SelectedUser.Username + "\" and all its data?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DeleteSelectedUser();
                }
            });
        }
    }
}
