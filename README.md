# Ye自用M3U8下载器

## 设计初衷

1.支持调用Aria2下载切片文件(且能自由控制其下载参数如并发数量等等)

	市面上有一些工具也是支持调用此工具下载的,但是可控性有BUG,又没有开源无法将其修复,用起来总是不舒服.
	防止出现使用 FFmpeg 下载时出现卡住不动,且无法断点续传的问题.

1.支持复制黏贴多个 M3U8 文件然后批量下载

	市面上找到的几款下载工具都只能一次一个的添加下载,对于下载多集电视剧时非常的麻烦.

1.支持自动合并切片(*.TS)文件

	市面上的工具貌似都支持这个功能.

## 源项目地址
https://github.com/asiontang/93.Ye.M3U8.Downloader  
https://gitee.com/AsionTang/93.Ye.M3U8.Downloader  

## TIPS
### 常用UserAgent查看地址：
https://developers.whatismybrowser.com/useragents/explore/operating_platform/iphone/
http://www.useragentstring.com/pages/useragentstring.php
### aria2最新版下载：
https://github.com/aria2/aria2/releases/
### ffmpeg最新版下载：
https://ffmpeg.zeranoe.com/builds/  

----------------------------------------------------------------------------------------------------

## 新版本说明
	93.Ye.M3U8.Downloader v2.0，个人在原版基础上修改、优化。程序运行需要.NET Framework 4.6或更高版本。

## 项目地址：
https://github.com/charleszhang97/93.Ye.M3U8.Downloader  
https://gitee.com/charles97/93.Ye.M3U8.Downloader
## 程序使用的WebUI项目地址：
https://github.com/ziahamza/webui-aria2  

## 其他说明：
1. 如果合并控制台的命令参数需要修改，修改FFmpeg/cmd.txt即可，程序启动会自动加载。

2. 支持输入本地m3u8文件的路径作为URL，支持拖曳文件到窗口。

	本地m3u8文件中不包含绝对网址时（key标记包含URI时会自动获取），请手动添加一个URI标记（程序获取后会与相对地址拼接成对应的下载网址）：  
	例：#URI:https://abc/d/e/

3. 根据不同的下载场景，选择不同的Aria2c配置文件（启动Aria2c前），FFmpeg文件夹中如果有同名txt，自动加载到控制台。

	例：bxg.aria2c.conf -->  bxg.aria2c.txt

4. useragent设定，默认选择windows-chrome，可以手动切换（下载开始前）。可以在config文件设定，会自动追加到列表并选中。

5. Download文件夹是默认下载路径，Data文件夹存放WebUI网页文档，Log文件夹保存程序运行时的控制台输出，settings.ini保存程序退出时的设定。

6. 参数追加，用于启动Aria2时追加参数，比如"--dir=D:\"。

7. 合并为任务组选项，用于下载非HLS任务时，比如多张图片需要保存到同一目录。
