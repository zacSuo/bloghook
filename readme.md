执行结果直接通过vs的IDE可以完美
但是通过IIS时候，status、add和log都可以执行成功，但pull和commit都不行，也没报错，只是执行后没有任何输出，然后看代码也没有更新
搞了一整天也没搞定，先放下吧
自己猜测的可能有两个原因，但经过尝试依旧无无法解决：

    1. 权限问题，也许IIS对git的某些操作或系统文件不够权限，git权限问题可能性小，毕竟git部分功能可用
        在IIS管理器中，为aspnet网站应用池设置为Administrator用户了
        在服务中，为IIS Admin服务也允许了跟桌面应用程序交互
    2.git账户问题，也许是pull和commit需要确认账户，但是不知道其如何确认身份
        设置了全局身份：git config --global user.name "";   git config --global user.email ""
        将管理员目录下的.ssh文件夹，拷贝到了所有用户目录下
        
最终的折中做法是，写个本地Service，每小时定期执行一次pull操作来更新，显然不是需要的，但先凑合用