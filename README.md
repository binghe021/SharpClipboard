# SharpClipboard
C# Clipboard Monitor

Not original code, all credit goes here: https://gist.github.com/glombard/79863174. I added some additional functionality and got it working with Cobalt Strike's execute-assembly.

在原开源项目基础上添加功能，监测到复制了m3u8链接时自动调用N_m3u8DL-CLI下载器进行下载。

# Blogpost
https://grumpy-sec.blogspot.com/2018/12/i-csharp-your-clipboard-contents.html

# References

https://stackoverflow.com/questions/621577/clipboard-event-c-sharp

https://stackoverflow.com/questions/17762037/error-while-trying-to-copy-string-to-clipboard

https://docs.microsoft.com/en-us/windows/desktop/dataxchg/wm-clipboardupdate

https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.wndproc?view=netframework-4.7.2