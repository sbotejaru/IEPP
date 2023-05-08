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

## This project consists out of two parts:
 1. Internet Explorer++ Web Browser - made in C# WPF with Chromium Embedded Framework
 2. Custom Avatars generation - made with StyleGAN, AnimeGANv3, MXNET-Face and DeepFace using PyTorch and Tensorflow.

## 1. Internet Explorer++

### Profile selection

Up to 5 profiles, each one with its own avatar and browser settings/bookmarks/history.

If one chooses not to have a custom avatar, his avatar will be the default cup of coffee.

<img src="https://i.imgur.com/FFjqs03.gif" width="800" alt="">

### Avatar generation

If the user wants a custom avatar, he will have to select his desired features/randomize them.

The features will be fed to the generation model and generate the desired avatar.

<img src="https://i.imgur.com/3cDRiIl.gif" width="800" alt="">


### Browsing

After the profile has been selected, its data will be loaded and the browser will be displayed.

The web page is displayed using the Chromium Embedded Framework.

<img src="https://i.imgur.com/bh7OWq6.gif" width="800" alt="">

### Details

Page title and favicon on each tab.

<img src="https://i.imgur.com/LoznIH3.gif" width="600">

Tab animation and subtle loading bar.

<img src="https://i.imgur.com/AijVtfe.gif" width="500">

## 2. Avatar generation

The photo-realistic generated images are done with the help of StyleGAN by Nvidia.

The StyleGAN takes as an input a Gaussian Distribution of 512 random float numbers, and outputs an image of a generated person.

<img src="https://i.imgur.com/1TOCgER.png" width="700">

The photos are fed into a feature extractor(DeepFace and MXNET-Face) that predicts the existing facial features in the image.

<img src="https://i.imgur.com/TvfYVYq.png" width="700">

The input vector and the extracted features are collected into a dataset and used as a training set for a linear regression, that predicts the latent space from the input features.

<img src="https://i.imgur.com/Oc1oQE5.png" width="700">
