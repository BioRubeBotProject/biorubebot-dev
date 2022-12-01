# BioRubeBot

#### About This Project
This project is an ongoing development of an education Biology game focused on biological cellular activity.

Dr. Sara Cline, Professor of Biology at Athens State University, envisioned a game that would introduce individuals to biological cellular activity.  BioRubeBot is an educational application developed to entertain and educate young and old alike.  Game play is designed to simulate the cellular Mitogen-activated Protein Kinase signalling process (MAP-K) and the Trimeric G-protein cell signaling cycle.

### Notes to the developers

Developers that work on this project will **NOT** have access to making changes to master.

### How to Build the Game Locally
1. Navigate to the most current version in GitHub
   * Based on current knowledge, the most up-to-date repository is:
     https://github.com/BioRubeBotProject/biorubebot-dev
2. At this point it is reccommended that you create the branches as listed below in "Notes to Developers: Github"
3. Use command `git clone <link-from-URL-above>` or download Github Desktop and use it to navigate and download your new -dev branch.
4. Make note of the location that the repository is downloaded to.  Any files you edit here can be committed to github.
5. Download Unity Hub.
6. Once installed, "Open Project" and navigate to your downloaded repository from step 4.
7. Unity Hub should identify the current unity version for this project, which will probably be Unity Version `2021.1.21.f1'
8. Unity Hub should allow you to download that specific version. You may then open this project.
   * It may be required to "Switch Target" based on the OS being developed on
   * Note: iOS may be developed on all platforms, but cannot be built without MacOS and XCode.  Unity will give you the XCode files for transfer.
   * School Macs and iPads may be used for testing purposes, and a Windows build can be given to Dr. Cline for her own testing purposes.


### Notes to developers: Github
It is recommended to make two primary branches for the duration of your team
1. Create a "production/pseudo-master" branch labeled `<Term(ex:Fall, Spring, Summer)><Year>-Production`, for example `Spring2022-Production`
2. Create a "staging" branch labeled `<Term(ex:Fall, Spring, Summer)><Year>-Dev`, for example `Spring2022-Dev`
3. Personal and feature branches can furthermore be created from these branches.

The `-Production` branch will be used as the submission branch at the end of the semester.

The `-Dev` branch will be used to bring together your teams changes *after testing has been done on individual branches* so that all teammates can have the current, working development branch.

Team members should also make unique branches for each feature to be worked on so that neither the `-Dev` or `-Production` branches become corrupted with potentially bad code.

It is suggested to follow the standard git practice of building individual feature branches.
Feature branches similarly mock story names as each branch should accomplish a story.

For more on how to use git and the etiquette, please review:
https://www.git-tower.com/learn/git/ebook/en/command-line/appendix/best-practices

The Project is currently compiled under Unity Version 2021.1.21.f1.

### Notes to developers: IDE
While the editing of script files can be done in any IDE, Unity specific intellisense and project refrences seem to work best with Visual Studio 2015.  This can be set under Unity's Preferences. 
