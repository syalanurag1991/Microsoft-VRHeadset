For Android, we need a file called fileList.txt to be packaged along with the
Affdex data files in StreamingAssets/affdex-data-3.

This file enumerates the Affdex data files, one file per line (just files,
no directories), and begins with a line that specifies the git commit hash
from the affdexface-data repo that corresponds to that specific set of
data files.

The listed files are relative to the StreamingAssets/affdex-data-3 directory,
and do not contain a leading "./"

Example file (abc123 is the commit hash)
----------------------------------------
abc123
foo.txt
somedir/bar.txt

To generate this file, cd into the StreamingAssets/affdex-data-3 folder and
run the unix command:

find .  -type f ! \( -name '*.meta' -o -name fileList.txt -o -name README.txt \) -printf '%P\n' > fileList.txt

Then, edit the file and add a line at the top which is the commit hash from
the affdexface-data repo that corresponds to the data files.

