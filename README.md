# 小熊渲染器PowerBearRender
（C）版权所有 collinmail@126.com（@大千小熊）

一个基于CPU的光线追踪离线渲染器。

<div style="color:yellow">
<p>本项目基于GPL 3.0开源协议进行编写，请务必注意版权所属规范。</p>
<p>如果您使用了本项目进行研究，那么请务必，申明本项目的开源链接和开源地址。</p>
<p>此外，请勿在未经允许的情况下，将软件进行商业化使用，同时，不允许学校去拿我的作品，给别的同学进行参加比赛。也不允许其他人再未经允许下投稿比赛，和商业化使用。</p>
<p style="color:red">注意：本项目拥有完善的著作权和版权，违规使用所造成的法律责任由您承担。</p>
<p>大陆地区，软件著作权申明。</p>
</div>

* CPU软件渲染器
* 相比于传统渲染器，使用更加简单
* 内存占用很小

# 技术要点
* PBR基于物理的渲染
* NPR卡通化渲染
* BVH包围盒体加速运算
* Asp.Net服务器多电脑网络联合运算

同时本项目也实现了一些专利方式的数据结果。例如自己设计的PbObj格式文件，可以持久化存储模型文件的修改，减小模型文件占用磁盘的体积。

## 其他技术：
* 抗锯齿算法MSAA
* 动态调整迭代次数的新型计算公式
* 三角面的光线渲染求交
* 和其他软件DCC配合使用
* CPU多线程渲染器实现

同时，本项目也使用了一些OpenCV库来辅助降噪。本项目的渲染器部分，不使用现成的OpenGL和Direct12，而是从头手写自研。

本软件也支持

## 软件结构概览
使用C#和C++混合编写。软件无需Runtime的支持，默认打包已经设定为带Runtime打包。
# 效果图
![Alt text](Docs/Images/%E8%8A%AD%E8%95%BE%E8%88%9E%E5%A5%B3%E5%AD%A9%E4%B8%AD%E9%97%B4%E5%9B%BE.png)


渲染一张人物模型，这个模型带多个材质，本项目实现了多个材质槽系统。


![Alt text](Docs/Images/%E5%96%89%E5%A4%B4.png)


渲染一张Blender标志性的模型，猴头，放地面上。


![Alt text](Docs/Images/%E5%B0%8F%E7%90%83%E4%BB%AC%E7%9A%84%E6%99%9A%E9%9C%9E.png)
从左到右是：玻璃材质、地球材质、金属材质。
# 下载和安装指南
现在您所处于的区域是，开源代码区域。

您应该`git clone`本项目，然后使用自己的代码编辑器打开本项目（使用VisualStudio2022软件）。

然后，设定运行时和目标，在VisualStudio上完成项目代码的编译和构建。

# 核心亮点

相比于传统的渲染器，本项目所使用的资源很小，兼容性广阔。

并且本项目从零手写，考虑到平常大部分使用需求并没有那么高，所以本项目内置了很多预设，可以方便用户一键出图，一键渲染动画，一键NPR卡通化渲染。

本项目基本实现了基本的渲染算法，更多优化思路和新的功能正在研究中。

---
本项目的文档正在编写中，有关于本项目的所有提交历史记录，请见commit记录。这是我编写这个软件的，所有心路。

# 开源组件使用申明
1. Windows Presentation Foundation（WPF）：Licensed Under MIT；
2. .NET Runtime：Licensed Under MIT；
3. OpenCV4：Licensed Under BSD；
4. Asp.Net Core：Licensed Under MIT；

本软件渲染部分实现只使用了C#和C++自带的一些STL库。