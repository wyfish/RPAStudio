REM 推送当前目录下的所有nupkg包到rpa-nexus.openserver.cn服务器上
for %%f in (*.nupkg) do (
    nuget push %%f 3488dc1d-cb31-36fc-a9ba-5d1463c43a53 -src http://rpa-nexus.openserver.cn/repository/rpa-community-activity/
)

pause