# MARC-2.0
Mobile Application Review Classifier 2.0
MARC 2.0 is the second release of our Mobile Application Review Classifier MARC. MARC 2.0 provides functionality for automatically classifying and summarizing user reviews on mobile application stores, an enhanced data classification engine, and a new GUI.

## Features

New and Improved User Interface

####Automatic User Review Extraction: MARC provides a feature for extracting recent user reviews from the iOS app store. Simply enter the app store ID for any app in the iOS app store and MARC will import upto 500 most recent user reviews.

####Text Processing Features

#####Stemming: MARC uses Porter Stemmer (http://www.tartarus.org/~martin/PorterStemmer) to reduce words to their morphological roots by removing derivational and inflectional suffixes.

#####Stop-word Removal: MARC provides a feature for removing English words that are considered too generic (eg. the, in, will).

#####Sentence Extraction: Since a single user review might include more than one maintainence request, MARC processes user reviews a sentence at a time.

####Automatic Model Saving: MARC builds Model on the first run and saves it so that the saved Model is used each time while classification. As a result, time is saved as Model is not built every time while classification. Saved model is used until training file is changed and a new model is automatically built. 

####Summarization: MARC has the capability to summarize the classified reviews. Classification algorithm includes Hybrid TF, Hybrid TFIDF, SumBasic, and LexRank
##Installation MARC requires .Net 4.5.2 and Java 1.8 to run. 

To be added in future:
MARC can be installed by running the installer from the following directory :

MARC Installer -> Debug -> MARC Installer.msi

MARC provides default training datasets (BOF Dataset.arff and BOW Dataset.arff) in the installation directory (C:\Program Files (x86)\LSU MARC\MARC 1.0 - Mobile App Review Classifier\InputData). You can either edit this training dataset or use one of your own. However, please make sure that the training dataset you use follows the same format as the default training dataset.

##Working on Source Code In order to open and modify the C# source project you will need:

Visual Studio 2015 Free Community Edition
.Net 4.5.2. Once you have loaded the project open MARC 1.0.sln in src directory in Visual Studio and select MARC as the startup project. You may also have to link references from the project directory.
