# WebGAN

### Avatar Generation Demo: https://www.kaggle.com/code/sbotejaru/avatar-gen-demo
Instructions:
1. Register/Login to Kaggle and verify your account.
2. Press "Edit" on the project.
3. Select a GPU accelerator (account must be verified) from the dots on the top right, and press "Run All"
4. Press +/- on each feature to modify the generated image.
5. If the demo freezes, reload the session from the top right.

## Demo examples:

<img src="https://i.imgur.com/zZmKi6P.gif" width="800" alt="">
<img src="https://i.imgur.com/bUWK7Yv.gif" width="800" alt="">
<img src="https://i.imgur.com/RIQ2dkc.gif" width="800" alt="">

## The project consists out of two parts:
 1. Internet Explorer++ Web Browser - made in C# WPF with Chromium Framework
 2. Custom Avatars generation - made with StyleGAN, AnimeGANv3, MXNET-Face and DeepFace using PyTorch and Tensorflow.

### 1. Internet Explorer++

**Profile selection**

Up to 5 profiles, each one with its own avatar and browser settings/bookmarks/history.

If one chooses not to have a custom avatar, his avatar will be the default cup of coffee.

<img src="https://i.imgur.com/FFjqs03.gif" width="800" alt="">

**Avatar generation**

If the user wants a custom avatar, he will have to select his desired features/randomize them.

The features will be fed to the generation model and generate the desired avatar.

<img src="https://i.imgur.com/3cDRiIl.gif" width="800" alt="">


**Browsing**

After the profile has been selected, its data will be loaded and the browser will be displayed.

<img src="https://i.imgur.com/bh7OWq6.gif" width="800" alt="">
