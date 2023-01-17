// See https://aka.ms/new-console-template for more information

using ACM.Git;

Git git = new("/home/linusberg/Development/test");

await git.Mirror("git://github.com/linus-berg/DrinkALot.git");