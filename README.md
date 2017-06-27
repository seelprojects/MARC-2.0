# Mobile Application Review Classifier (MARC) 2.0


MARC 2.0 is the second release of our [Mobile Application Review Classifier MARC](https://github.com/seelprojects/MARC). MARC 2.0 provides functionality for automatically classifying and summarizing user reviews on mobile application stores, an enhanced data classification engine, and a new GUI.

![ScreenShot](https://github.com/seelprojects/MARC-2.0/blob/master/MARC%202.0/MARC.PNG)

# Summarization

MARC 2.0 provides multiple data summarization algorithms that can be used to generate concise and comprehensive summaries of user reviews. These algorithms include Hybrid TF, Hybrid TFIDF, SumBasic, and LexRank.

# Installation

MARC requires .Net 4.5.2 and Java 1.8 to run. MARC can be installed by running the installer from the directory: MARC Installer -> Debug -> MARC 2.0 Installer.msi, or [Click Here!](https://github.com/seelprojects/MARC-2.0/blob/master/MARC%202.0%20Installer/Debug/MARC%202.0%20Installer.msi)

MARC provides default training datasets (BOF Dataset.arff and BOW Dataset.arff) in the local app data installation directory (C:\Users\{Username}\AppData\Roaming\MARC 2.0\InputData). You can either edit this training dataset or use one of your own. However, please make sure that the training dataset you use follows the same format as the default training dataset.

# Modification

In order to open and modify the C# source project you will need [Visual Studio 2015, FreeCommunity Edition](https://www.visualstudio.com/vs/community/) .Net 4.5.2. Once you have loaded the project open MARC 2.0.sln in src directory in Visual Studio and select MARC as the startup project. You may also have to link references from the project directory.

# License

Please refer to the file LICENSE.md for license information.

# Disclaimer

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
