using IEPP.Controls;
using IEPP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private string usersDir;

        private Visibility vis;

        public Visibility Vis
        {
            get { return vis; }
            set { vis = value; NotifyPropertyChanged("Vis"); }
        }

        private void CreateAppDirectory()
        {
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string subPath = "/Internet Explorer++";
            workingDir = docsFolder + subPath;

            bool exists = Directory.Exists(workingDir);

            if (!exists)
                Directory.CreateDirectory(workingDir);
        }

        private void CreateUsersDirectory()
        {
            usersDir = workingDir + "/Users";
            bool exists = Directory.Exists(usersDir);

            if (!exists)
                Directory.CreateDirectory(usersDir);
        }

        private void CreateNewUserDirectory(string username, Uri avatarSrc) // + image
        {
            var newUserDir = usersDir + "/" + username;
            bool exists = Directory.Exists(newUserDir);

            if (!exists)
            {
                Directory.CreateDirectory(newUserDir);
                var userAvatarPath = newUserDir + "/avatar.jpg";
                SaveAvatar(userAvatarPath, avatarSrc);
            }
        }

        private void SaveAvatar(string avatarPath, Uri avatarSrc)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(avatarSrc));
            using (FileStream stream = new FileStream(avatarPath, FileMode.Create))
                encoder.Save(stream);
        }

        private void LoadUsers()
        {
            string[] users = Directory.GetDirectories(usersDir);
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
            var userPath = usersDir + "/" + username;
            var avatarPath = new Uri(userPath + "/avatar.jpg");
            UserList.Add(new UserContainer() { Username = username, ProfilePic = new BitmapImage(avatarPath) });
        }

        private void InitAttrbiuteValues()
        {
            SelectedGender = -1;
            SelectedEthnicity = -1;
            SelectedHairColor = -1;
            SelectedHairStyle = -1;
        }

        private void SetVisToChooseList()
        {
            HideErrors();
            ChooseProfileVisibility = Visibility.Visible;
            NewProfileVisibility = Visibility.Collapsed;
        }

        private void HideErrors()
        {
            NewProfileError = Visibility.Hidden;
        }

        // Init function that sets all values to null / resets everything
        private void Init()
        {
            HideErrors();
            ChooseProfileVisibility = Visibility.Collapsed;
            NewProfileVisibility = Visibility.Collapsed;
            CreateAvatarVisibility = Visibility.Collapsed;
            UserList = new ObservableCollection<UserContainer>();
            CreateAppDirectory();
            CreateUsersDirectory();
            LoadUsers();
            InitAttrbiuteValues();
            NewUsername = "";
            NewProfileErrorMessage = "A profile with that name already exists!";
            FacialHairOpacity = 1;
            HairAttributesOpacity = 1;
        }

        private void RandomizeAttributes()
        {
            Random random = new Random();

            int RandomGender = random.Next(0, 2);
            int RandomEthnicity = random.Next(0, 6);
            int RandomAge = random.Next(1, 60);
            int RandomHairColor = random.Next(0, 4);
            int RandomHairStyle = random.Next(0, 2);
            int RandomMoustache = random.Next(0, 2);
            int RandomBeard = random.Next(0, 2);
            int RandomAccessory = random.Next(0, 2);
            int RandomBald = random.Next(0, 2);
            int RandomBangs = random.Next(0, 2);

            SelectedEthnicity = RandomEthnicity;
            SelectedAge = RandomAge;
            SelectedHairColor = RandomHairColor;
            SelectedHairStyle = RandomHairStyle;
            HasMoustache = true ? RandomMoustache == 1 : RandomMoustache == 0;
            HasBangs = true ? RandomBangs == 1 : RandomBangs == 0;
            HasGlasses = true ? RandomAccessory == 1 : RandomAccessory == 0;
            HasBeard = true ? RandomBeard == 1 : RandomBeard == 0;
            IsBald = true ? RandomBald == 1 : RandomBald == 0;
            SelectedGender = RandomGender;
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
            set { selectedUser = value; NotifyPropertyChanged("SelectedUser"); Console.WriteLine(selectedUser.Username); Vis = Visibility.Collapsed; }
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

        private int selectedEthnicity;
        public int SelectedEthnicity
        {
            get { return selectedEthnicity; }
            set { selectedEthnicity = value; NotifyPropertyChanged("SelectedEthnicity"); }
        }

        private int selectedHairColor;
        public int SelectedHairColor
        {
            get { return selectedHairColor; }
            set { selectedHairColor = value; NotifyPropertyChanged("SelectedHairColor"); }
        }

        private int selectedHairStyle;
        public int SelectedHairStyle
        {
            get { return selectedHairStyle; }
            set { selectedHairStyle = value; NotifyPropertyChanged("SelectedHairStyle"); }
        }

        private int selectedAge;
        public int SelectedAge
        {
            get { return selectedAge; }
            set { selectedAge = value; NotifyPropertyChanged("SelectedAge"); }
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
            set { hasBangs = value; NotifyPropertyChanged("HasBangs"); }
        }

        private bool hasGlasses;
        public bool HasGlasses
        {
            get { return hasGlasses; }
            set { hasGlasses = value; NotifyPropertyChanged("HasGlasses"); }
        }

        private bool hasBeard;
        public bool HasBeard
        {
            get { return hasBeard; }
            set { hasBeard = value; NotifyPropertyChanged("HasBeard"); }
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
                    FacialHairOpacity = 0.5;
                }
                else
                {
                    FacialHairOpacity = 1;
                }

                NotifyPropertyChanged("IsFemale");
            }
        }


        #endregion

        public ObservableCollection<UserContainer> UserList { get; set; }
        public RelayCommand AddNewProfileCommand { get; set; }
        public RelayCommand SaveProfileCommand { get; set; }
        public RelayCommand CreateAvatarCommand { get; set; }
        public RelayCommand SaveAvatarCommand { get; set; }
        public RelayCommand GenerateAvatarCommand { get; set; }
        public RelayCommand RandomizeAvatarCommand { get; set; }

        public ChooseProfileVM()
        {
            Init();

            var icon = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/coffee.jpg");

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

                    bool exists = InUserList(NewUsername);

                    if (!exists)
                    {
                        CreateNewUserDirectory(NewUsername, icon);
                        AddToUserList(NewUsername);
                        NewUsername = "";

                        if (UserList.Count == 5)
                            AddNewVisibility = Visibility.Collapsed;

                        HideErrors();
                        SetVisToChooseList();
                    }
                    else
                    {
                        NewProfileErrorMessage = "A profile with that name already exists!";
                        NewProfileError = Visibility.Visible;
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
                CreateAvatarVisibility = Visibility.Collapsed;
                NewProfileVisibility = Visibility.Visible;
            });

            GenerateAvatarCommand = new RelayCommand(o =>
            {
                // handle attributes function
            });

            RandomizeAvatarCommand = new RelayCommand(o =>
            {
                RandomizeAttributes();
            });
        }
    }
}
