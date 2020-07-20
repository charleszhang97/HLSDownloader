:: 目的：
:: 		为了减少体积，创建自解压7Z包，因为原 ffmpeg.exe 达到 60多MB，太大了。
:: 		现在经过处理，减小到14MB左右，同时能无感解压并执行传递的参数。
:: 		牺牲运行时间来换取磁盘空间。
:: 
:: 参考资料：
:: 		* [7z制作自解压安装包_慕课手记](https://www.imooc.com/article/28626) 
::			然后需要下载 7zS2con.sfx 文件。
::			7zs.sfx文件是7z为制作自解压的安装程序提供的一个文件。
::			9.20的7zs.sfx文件在7-Zip extra包中，之后的版本都把这个文件放在了LZMA包中，并且改名为7zs2.sfx
::			LZMA包下载地址：https://www.7-zip.org/sdk.html
::			lzma.7z\Bin\7zS2.sfx    适合具有UI的程序，假如用来封装 ffmpeg 则启动时会启动另外一个CMD窗口来执行额外参数
::			lzma.7z\Bin\7zS2con.sfx 适合命令行程序，假如用来封装 ffmpeg 则会在当前CMD窗口继续执行就像没有被压缩一样执行，仅仅在执行时卡几秒（正在解压）
::
::			[7z/Installer at master · sparanoid/7z · GitHub](https://github.com/sparanoid/7z/tree/master/Installer) 
::			精简版 SFX 模块与普通的 SFX 模块相似。但有下列区别：
::			 - 没有安装程序配置文件
::			 copy /b 7zS2.sfx + archive.7z sfx.exe
::			7zS2.sfx  small SFX module for installers (GUI version)  
::			7zS2con.sfx small SFX module for installers (Console version) 
::			您可以对安装包发送参数请求，然后安装包会将它们传送到指定的 .exe 文件
::			精简版 SFX 模块有三个优先级规则来决定哪个文件被自动运行
@COPY /B 7zS2con.sfx + ffmpeg.7z ffmpeg.exe