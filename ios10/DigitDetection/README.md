---
name: Xamarin.iOS - MPSCNNHelloWorld
description: "This sample is a port of the open source library, TensorFlow trained networks trained on MNIST Dataset via inference... (iOS10)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios10
urlFragment: ios10-digitdetection
---
# MPSCNNHelloWorld

This sample is a port of the open source library, `TensorFlow` trained networks trained on [MNIST Dataset](http://yann.lecun.com/exdb/mnist/) via inference using Metal Performance Shaders.  
The sample demonstrates how to encode different layers to the GPU and perform image recognition using trained parameters (weights and bias) that have been fetched from, pre-trained and saved network on TensorFlow.

The Single Network can be found [here](https://www.tensorflow.org/versions/r0.8/tutorials/mnist/beginners/index.html#mnist-for-ml-beginners)
The Deep Network can be found [here](https://www.tensorflow.org/versions/r0.8/tutorials/mnist/pros/index.html#deep-mnist-for-experts)

The network parameters are stored a binary `.dat` files that are memory-mapped when needed.

![MPSCNNHelloWorld application screenshot](Screenshots/Main.png "MPSCNNHelloWorld application screenshot")

## Build Requirements

Building this sample requires Xcode 8.0 and iOS 10.0 SDK

## Refs

- [Original sample](https://developer.apple.com/library/prerelease/content/samplecode/MPSCNNHelloWorld/Introduction/Intro.html#//apple_ref/doc/uid/TP40017482)
- [Intro to neural networks](http://neuralnetworksanddeeplearning.com/)

## Target

This sample runnable on iPhone/iPad with following features:

- iOS GPU Family 2 v1
- iOS GPU Family 2 v2
- iOS GPU Family 3 v1

## Licesnse

Xamarin port changes are released under the MIT license.
