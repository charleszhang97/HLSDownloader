
Ye自用M3U8下载器

设计初衷
1.支持调用Aria2下载切片文件(且能自由控制其下载参数如并发数量等等)

市面上有一些工具也是支持调用此工具下载的,但是可控性有BUG,又没有开源无法将其修复,用起来总是不舒服.
防止出现使用 FFmpeg 下载时出现卡住不动,且无法断点续传的问题.

2.支持复制黏贴多个 M3U8 文件然后批量下载

市面上找到的几款下载工具都只能一次一个的添加下载,对于下载多集电视剧时非常的麻烦.

3.支持自动合并切片(*.TS)文件

市面上的工具貌似都支持这个功能.


项目地址：
https://github.com/asiontang/93.Ye.M3U8.Downloader
https://gitee.com/AsionTang/93.Ye.M3U8.Downloader

TIPS:
常用UserAgent查看地址：
https://developers.whatismybrowser.com/useragents/explore/operating_platform/iphone/
http://www.useragentstring.com/pages/useragentstring.php
aria2最新版下载：
https://github.com/aria2/aria2/releases/
ffmpeg最新版下载：
https://ffmpeg.zeranoe.com/builds/

----------------------------------------------------------------------------------------------------

	93.Ye.M3U8.Downloader v2.0，个人在原版基础上修改、优化。程序运行需要.NET Framework 4.6或更高版本。

项目地址：
https://github.com/charleszhang97/93.Ye.M3U8.Downloader
https://gitee.com/charles97/93.Ye.M3U8.Downloader
WebUI：
程序使用的WebUI项目地址如下，已经内嵌到程序中（程序有660K大小是因为webui-aria2.zip文件）。
https://github.com/ziahamza/webui-aria2

其他说明：
1、如果合并控制台的命令参数需要修改，修改FFmpeg/cmd.txt即可，程序启动会自动加载。
2、支持输入本地m3u8文件的路径作为URL，支持拖曳文件到窗口。
	本地m3u8文件中不包含绝对网址时（key标记包含URI时会自动获取），请手动添加一个URI标记（程序获取后会与相对地址拼接成对应的下载网址）：
	例：#URI:https://abc/d/e/
3、根据不同的下载场景，选择不同的Aria2c配置文件（启动Aria2c前），FFmpeg文件夹中如果有同名txt，自动加载到控制台。
	例：bxg.aria2c.conf -->  bxg.aria2c.txt
4、useragent设定，默认选择windows-chrome，可以手动切换（下载开始前）。可以在config文件设定，会自动追加到列表并选中。
5、Download文件夹是默认下载路径，Data文件夹存放WebUI网页文档，Log文件夹保存程序运行时的控制台输出，settings.ini保存程序退出时的设定。
6、参数追加，用于启动Aria2时追加参数，比如"--dir=D:\"。
7、合并为任务组选项，用于下载非HLS任务时，比如多张图片需要保存到同一目录。

参数调用：
	共支持五个参数：config-name、aria2c-args-append、max-speed-limit、name-url-string、action-after-downloaded，分别对应Aria2配置文件名、Aria2命令参数追加、全局下载速度限制、文件名与下载地址key:value字符串、下载完成后操作。
	参数可以无序，格式为 参数名=参数值，参数值有空格的话需要套上引号。参数设定等同于手动操作，但方便外部调用，比如一键下载。
	aria2c-args-append的参数值参考Aria2的命令行参数给定，举例：aria2c-args-append="--dir=D:\ --referer=www.xxx.com"。
	max-speed-limit值为整型，单位固定为k/s，例如1024表示设定全局下载速度最大1m/s。
	name-url-string，值的格式 文件名1:地址1;文件名2:地址2;... ，分隔符为:和;（英文）。
	action-after-downloaded，值可以设定为 关闭Aria2、退出程序、关机等，其他值参考程序下载完操作下拉框。

浏览器调用：
	注册表进行设定后（百度、Google），可以支持URL Protocol协议进行调用。不同于上面参数调用的是，所有参数需要拼接为一个长字符串（竖线分隔），作为参数传入。
	补充说明的是，所有参数值需要经过Base64编码，这样是为了避免中文等参数传输出现问题。参考程序目录“Services\油猴脚本参考.js”。
	比如我的实现方法，浏览器扩展启用油猴脚本，写一个匹配指定网站的脚本，抓取m3u8文件地址，放到a标签href，显示到页面上。点击后，浏览器调用本程序，传入参数。
	a标签html最终格式可能如下（协议头可以自定义，没有限定；参数值为空的参数可以省略）：
	<a href="M3u8DL://config-name=YXJpYTJjLmNvbmY=|aria2c-args-append=|max-speed-limit=|name-url-string=VGVzdA==:aHR0cHM6Ly93d3cueHh4LmNvbS9obHMvaW5kZXgubTN1OA==|action-after-downloaded=5YWz6ZetQXJpYTI=">下载视频</a>
	点击上述超链接，href的值会作为一个参数来启动程序，程序设定配置文件aria2c.conf，开始下载https://www.xxx.com/hls/index.m3u8，最终保存为Test.mp4，然后关闭Aria2。
	浏览器调用，会变更程序的工作目录，如火狐是System32目录、谷歌是自己的安装目录，这会造成程序调用不了自身依赖的DLL。写了一个m3u8dl.service.exe作为中间程序，注册表中程序地址写“~\Services\m3u8dl.service.exe”即可解决，参数格式不变。

扩展说明：
	下载地址框粘贴URL，会自动生成临时文件名，双击临时文件名会选中，可以再粘贴文件名。
	合并控制台，批处理可以写上合并成功后Explorer打开选中，或者Potplayer播放"{文件名}.mp4"，一步到位。
