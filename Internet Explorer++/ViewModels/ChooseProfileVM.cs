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

        private void CreateNewUserDirectory(string username, Uri avatarSrc) // + image
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
            LoadingScreenVisibility = Visibility.Collapsed;
            ChooseProfileVisibility = Visibility.Collapsed;
            NewProfileVisibility = Visibility.Collapsed;
            CreateAvatarVisibility = Visibility.Collapsed;
            UserList = new ObservableCollection<UserContainer>();
            NewUsername = "";
            NewProfileErrorMessage = "A profile with that name already exists!";
            FacialHairOpacity = 1;
            HairAttributesOpacity = 1;
            AvatarImage = new BitmapImage(new Uri("\\Icons\\avatar_placeholder.png", UriKind.Relative));
            LoadingText = "";            

            HideErrors();
            InitAttrbiuteValues();
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

                if (selectedUser.Username != "")
                    CurrentUserPath = WorkingDir + "/" + selectedUser.Username;
                else
                    CurrentUserPath = "";
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

        private string avatarPlaceholderImage;
        public string AvatarPlaceholderImage
        {
            get { return avatarPlaceholderImage; }
            set { avatarPlaceholderImage = value; NotifyPropertyChanged("AvatarPlaceholderImage"); }
        }

        private ImageSource generatedImage;
        public ImageSource AvatarImage
        {
            get { return generatedImage; }
            set { generatedImage = value; NotifyPropertyChanged("AvatarImage"); }
        }

        private Visibility loadingScreenVisibility;
        public Visibility LoadingScreenVisibility
        {
            get { return loadingScreenVisibility; }
            set { loadingScreenVisibility = value; NotifyPropertyChanged("LoadingScreenVisibility"); }
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

        private void CreateFeaturesFile()
        {
            List<double> featureList = new List<double>();
            var random = new Random();

            double female = IsFemale ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01;

            featureList.Add(female);
            featureList.Add(1 - female);

            for (int index = 0; index < 6; ++index)
                featureList.Add(random.Next(1, 30) * 0.01);

            featureList[SelectedEthnicity + 2] = random.Next(70, 101) * 0.01;

            featureList.Add(IsBald ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01);
            // if isbald generate small values for bangs, hair colors, hair length and density
            featureList.Add(HasBangs ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01);

            for (int index = 0; index < 5; ++index)
                featureList.Add(random.Next(1, 30) * 0.01);

            featureList[13] = HasGlasses ? random.Next(70, 101) * 0.01 : random.Next(1, 30) * 0.01;

            if (SelectedHairColor != (int)HairColor.Gray)
                featureList[SelectedHairColor + 10] = random.Next(70, 101) * 0.01;
            else
                featureList[14] = random.Next(70, 101) * 0.01;

            featureList.Add(random.Next(1, 51) * 0.01);

            featureList.Add(HasBeard ? random.Next(1, 30) * 0.01 : random.Next(70, 101) * 0.01);

            switch (SelectedHairStyle)
            {
                case (int)HairLength.Short:
                    if (IsFemale)
                    {
                        featureList.Add(random.Next(25, 51) * 0.01);
                        featureList.Add(random.Next(25, 51) * 0.01);
                    }
                    else
                    {
                        featureList.Add(random.Next(0, 21) * 0.01);
                        featureList.Add(random.Next(00, 21) * 0.01);
                    }
                    break;
                case (int)HairLength.Medium:
                    if (IsFemale)
                    {
                        featureList.Add(random.Next(50, 76) * 0.01);
                        featureList.Add(random.Next(50, 76) * 0.01);
                    }
                    else
                    {
                        featureList.Add(random.Next(20, 41) * 0.01);
                        featureList.Add(random.Next(20, 41) * 0.01);
                    }
                    break;
                case (int)HairLength.Long:
                    if (IsFemale)
                    {
                        featureList.Add(random.Next(50, 76) * 0.01);
                        featureList.Add(random.Next(75, 101) * 0.01);
                    }
                    else
                    {
                        featureList.Add(random.Next(20, 41) * 0.01);
                        featureList.Add(random.Next(41, 61) * 0.01);
                    }
                    break;
                default:
                    if (IsFemale)
                    {
                        featureList.Add(random.Next(50, 76) * 0.01);
                        featureList.Add(random.Next(50, 76) * 0.01);
                    }
                    else
                    {
                        featureList.Add(random.Next(20, 41) * 0.01);
                        featureList.Add(random.Next(20, 41) * 0.01);
                    }
                    break;
            }

            featureList.Add((100 - SelectedAge) * 0.01);

            File.WriteAllLines("feature_list.txt", featureList.Select(x => x.ToString()));
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
            switch(ProcessStep)
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
                pProcess.StartInfo.RedirectStandardOutput = true;
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

            Console.WriteLine("generated");
        }

        public ObservableCollection<UserContainer> UserList { get; set; }
        public RelayCommand AddNewProfileCommand { get; set; }
        public RelayCommand GuestCommand { get; set; }
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
        }
    }
}
