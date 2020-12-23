# HLSDownloader v2.0.6.0

## 原作者说明

>### Ye自用M3U8下载器

>>#### 设计初衷

>>>1.支持调用Aria2下载切片文件(且能自由控制其下载参数如并发数量等等)

>>>```市面上有一些工具也是支持调用此工具下载的,但是可控性有BUG,又没有开源无法将其修复,用起来总是不舒服.```
>>>```防止出现使用 FFmpeg 下载时出现卡住不动,且无法断点续传的问题.```

>>>2.支持复制黏贴多个 M3U8 文件然后批量下载

>>>```市面上找到的几款下载工具都只能一次一个的添加下载,对于下载多集电视剧时非常的麻烦.```

>>>3.支持自动合并切片(*.TS)文件

>>>```市面上的工具貌似都支持这个功能.```

## 原作者项目地址
https://github.com/asiontang/93.Ye.M3U8.Downloader  
https://gitee.com/AsionTang/93.Ye.M3U8.Downloader  

## 相关TIPS

### 常用UserAgent查看地址
https://developers.whatismybrowser.com/useragents/explore/  
http://www.useragentstring.com/pages/useragentstring.php  

### aria2最新版下载
https://github.com/aria2/aria2/releases/  

### ffmpeg最新版下载
https://ffmpeg.org/download.html#build-windows  

----------------------------------------------------------------------------------------------------

## 新版本说明
	93.Ye.M3U8.Downloader v2.0，更名为HLSDownloader，个人在原版基础上修改、优化。程序运行需要.NET Framework 4.6或更高版本。仅支持点播HLS视频，不支持直播类视频。另外，程序使用XML-RPC与Aria2通信，不是常见的JSON-RPC。除了HLS链接(`http://xxx/xxx.m3u8`)，还支持其他各种下载链接，本程序支持的所有URL类型参考使用说明15。

## 项目地址
https://github.com/charleszhang97/HLSDownloader  
https://gitee.com/charles97/HLSDownloader  

## WebUI
程序使用的WebUI项目地址如下： 
https://github.com/ziahamza/webui-aria2  
https://github.com/mayswind/AriaNg  

## ED2K
如果需要程序处理ED2K链接，则需要下载Windows版aMule：
https://github.com/kurniliya/amule-for-windows/releases  

## 使用说明
1. 如果合并控制台的命令参数需要修改，修改FFmpeg/cmd.txt即可，程序启动会自动加载。

2. 下载地址框粘贴m3u8地址，点击下载，程序会开始下载hls对应的m3u8、key、ts文件，下载完成时自动合并成视频。支持输入本地m3u8文件的路径作为URL，支持拖曳文件到窗口。

	本地m3u8文件中不包含绝对网址时（key标记包含URI时会自动获取），请手动添加一个BASE-URI标记（程序获取后会与相对地址拼接成对应的下载网址）：  
	例：`#BASE-URI:https://abc/d/e/`

3. 根据不同的下载场景，选择不同的Aria2c配置文件（启动Aria2c前），FFmpeg文件夹中如果有同名txt，自动加载到合并控制台。  

	例：`test.aria2c.conf  -->  test.aria2c.txt`

4. useragent设定，默认选择windows-chrome，可以随时进行切换。可以在config文件设定，会自动追加到列表并选中。

5. Download文件夹是默认下载路径，Data文件夹存放WebUI网页文档，Log文件夹保存程序运行时的控制台输出，Resource存放图片及声音资源文件，settings.ini保存程序退出时的设定。

6. 参数追加，用于启动Aria2时追加参数，比如"--dir=D:\"。

7. 合并为任务组选项，用于下载非HLS任务时，比如多张图片需要保存到同一目录。

8. settings.ini最后五项参数，可以按需要调整，有中文说明。

9. 下载并行任务数，输入整数后按Enter生效，没有按钮。

10. Aria2参数追加，除了对Aria2生效外，--user-agent、--referer、--header三个参数会自动应用到HLS任务所有切片，--peer-agent应用到BitTorrent下载任务，--http、--ftp、--bt、--metalink等起头的参数自动应用到所有下载任务，其他Aria2参数将不会应用于下载任务（可能是Aria2私有参数，不适用于下载任务）。注意，这里的应用只对后续任务生效，之前添加的任务不受影响。

11. 手动合并时，如果HLS任务的切片不完整，会自动统计出缺失的切片清单。

12. 手动合并（以及自动合并）生成批处理脚本时，以系统默认编码保存，如果有Unicode字符会出现乱码，这时程序自动以UTF-8保存bat文件，并添加一行“CHCP 65001”到头部，保证批处理能执行（echo等命令可能显示异常，与CMD窗口的默认字体设定有关）。

13. 最大上传速度限制，没有专门输入框，方法是在最大下载速度输入框输入整数后，“应用”按钮上右键，即可应用最大上传速度。

14. 右键快捷菜单以及快捷键，与点击相关按钮的功能是一样的。

15. 程序设计初衷是为了下载HLS(m3u8)视频，但是根据Aria2的功能特性，做了全面的功能扩展。除了HLS任务，还支持Http(s)、(S)FTP、任务组，以及磁力magnet、种子BitTorrent(.torrent)、Metalink(.metalink/.meta4)，thunder地址自动解析下载，ed2k自动调用aMule下载（需要下载aMule，放到Service\aMule，Aria2不支持ed2k），同时支持自定义正则以自动截取目标URL（参考17）。理论上来说，粘贴URL后能自动生成临时文件名的链接，程序都是支持的。

16. 打开WebUI，如果Data\open-webui.bat批处理存在，自动执行，程序默认行为会自动停止，适用于更换默认的WebUI这类情况；对于非HLS任务，如果其保存目录（任务下载目录）下download-completed.bat存在，自动执行，参数传入`"filename=XXX" "url=XXX" "size=XXX"`三个参数；全部下载完成触发时，如果下载（根）目录下all-downloads-completed.bat存在，则自动执行，参数传入`"totalnumber=XXX" "totaldata=XXX" "totaltime=XXX"`三个参数；具体使用方法，可以创建测试批处理脚本，使用%1、%2、%3等获取然后打印传入的参数。

17. 如果在程序启动目录下新建CustomRegexs.txt，输入URL抓取正则（一行一条正则表达式，分别匹配不同网站），启动程序后在下载地址框粘贴URL时会自动截取目标URL。正则文本举例：`http://.*path=(?<CaptureURL>[^&=]+).*`，程序会自动获取path参数的值作为下载网址，注意其中的`(?<CaptureURL>XXX)`格式必须包含，不然没有效果。

18. 下载地址框一行文本的格式为：文件名|下载地址，如果不是这个格式，点下载按钮时会被跳过。这是因为我写的是通用格式验证，不识别特定下载类型，如果每种类型分开验证太过麻烦，所以尽管有些任务“文件名”是被忽略的，但是点下载前必须保留程序自动生成的临时文件名。

19. 本地的.torrent、.metalink/.meta4等格式文件，可以直接拖到程序窗口，点击“下载”即可下载，当然也可以手动输入本地路径。

20. 下载完成后操作选项，容易被忽略，尤其是上次任务完成后执行了“关机”等危险性操作，程序启动后读取配置自动设置，容易出现意外，需要特别注意。

## 参数调用
* 共支持六个参数：startup、config-name、aria2c-args-append、max-speed-limit、name-url-string、action-after-downloaded，分别对应程序窗口状态、Aria2配置文件名、Aria2命令参数追加、全局下载速度限制、文件名与下载地址key:value字符串、下载完成后操作。

* 参数可以无序，格式为 参数名=参数值，参数值有空格的话需要套上引号。参数设定等同于手动操作，但方便外部调用，比如一键下载。

* startup，可选择的参数值共五个：normal、min、max、tray、exit。

* aria2c-args-append的参数值参考Aria2的命令行参数给定，举例：`aria2c-args-append="--dir=D:\ --referer=www.xxx.com"`。

* max-speed-limit值为整型，单位固定为k/s，例如1024表示设定全局下载速度最大1m/s。

* name-url-string，值的格式 `文件名1:地址1;文件名2:地址2;...` ，分隔符为:和;（英文）。

* action-after-downloaded，值可以设定为 关闭Aria2、退出程序、关机等，其他值参考程序下载完操作下拉框。

* 特殊参数decode，默认值为false，一般不需指定，除非输入的参数值经过base64编码；URL Protocol进行调用自动添加decode=true。

## 浏览器调用

* 注册表进行设定后（百度、Google），可以支持URL Protocol协议进行调用。不同于上面参数调用的是，所有参数需要拼接为一个长字符串（竖线分隔），作为参数传入。

* 补充说明的是，所有参数值需要经过Base64编码，这样是为了避免中文等参数传输出现问题。参考程序目录“Service\油猴脚本参考.js”。

* 比如我的实现方法，浏览器扩展启用油猴脚本，写一个匹配指定网站的脚本，抓取m3u8文件地址，放到a标签href，显示到页面上。点击后，浏览器调用本程序，传入参数。

* a标签html最终格式可能如下（协议头可以自定义，没有限定；参数值为空的参数可以省略）：
  `<a href="M3u8DL://config-name=YXJpYTJjLmNvbmY=|aria2c-args-append=|max-speed-limit=|name-url-string=VGVzdA==:aHR0cHM6Ly93d3cueHh4LmNvbS9obHMvaW5kZXgubTN1OA==|action-after-downloaded=5YWz6ZetQXJpYTI=">下载视频</a>`  
  点击上述超链接，href的值会作为一个参数来启动程序，最终执行效果为：程序自动选择配置文件aria2c.conf，开始下载`https://www.xxx.com/hls/index.m3u8`，最终视频保存为Test.mp4，然后关闭Aria2。

* 需要补充说明的是，浏览器调用会变更程序的工作目录，如火狐是System32目录、谷歌是自己的安装目录，这会造成程序调用不了自身依赖的DLL。写了一个m3u8dl.service.exe作为中间程序，注册表中程序地址写“~\Service\m3u8dl.service.exe”即可解决，参数格式不变。

## 扩展说明

* 下载地址框粘贴URL，会自动生成临时文件名，双击临时文件名会选中，可以再粘贴文件名。点击地址框，如果文本为空，自动读取剪切板。

* 非HLS任务如普通下载任务，可以强制指定保存的文件名，竖线|前“文件名”输入一个带后缀的文件名即可，适用于URL不包含文件名以文件流传输的下载地址。

* 合并控制台，批处理可以写上合并成功后Explorer打开选中，或者Potplayer播放"{文件名}.mp4"，以及其他自定义命令。

* 将其他目录下的aria2c.exe或者ffmpeg.exe拖到程序窗口，可以临时更改程序对二者的本地调用地址，退出后还原为默认，默认为Aria2、FFmpeg两个目录下相应的可执行程序。

* Resource目录下Arrow.png、download-complete.wav可以使用同名文件替换，有自定义需要时可尝试。

* 自带的WebUI，Data\webui-aria2.zip文件，经过本人修改webui-aria2\docs\app.js，用以获取传入的token参数，实现免输入，如果重新下载注意这里的不同之处。

* 第三方程序如果有最新版，可以下载后替换，如果替换后程序运行出现问题，可以还原为程序自带的版本。

* 注意Aria2不要启用SSL/TLS加密，即conf文件rpc-secure=true，这会导致程序无法与Aria2通信，即使rpc-certificate、rpc-private-key配置好也不行，程序内没有相应的处理。

* 启动Aria2后，正在下载一些任务，然后“设置下载目录”，添加新下载任务，之后又执行了“取消剩余下载”，这种情况下，可能会将之前的下载目录整体提示删除（会有【】包在路径前后），注意仔细确认。这是因为程序只认当前下载目录，其他目录不视为“下载目录”，分析任务目录时可能误将前下载目录当作任务目录。

* 程序没有历史记录功能，主要是合并控制台的输出文件地址不固定，无法捕获，只统计文件名和下载地址又无意义。需要查看下载历史，点击“打开下载目录”，通过文件管理器浏览。