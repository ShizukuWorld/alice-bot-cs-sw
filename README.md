# alice-bot-cs-sw
一个基于Mirai与MiraiCS与MiraiHttp的QQ机器人，雫世界构建 / A QQ Bot Named Alice Running with Mirai, ShizukuWorld Edition

[旧社区项目](https://github.com/MeowCatZ/alice-bot-cs-ce)

## 概述
- `AliceBot`是继`Urara`后由很菜很菜的`MashiroSA`发起的一个自研QQ机器人项目，使用`c#`编写，基于`Mirai(Mirai Core、Mirai Console、Mirai Http、Mirai CSharp)`。该版本为ShizukuWorld发行版
- `AliceBot`是运行在非盈利的基础上，一切行为仅供交流学习
- 该repo的`AliceBot`为社区发行版，开源构建，请勿贩售代码，不建议盈利行为
- 请注意，该bot依赖MiraiHttp作为中间件，版本为1.x，当前切勿使用2.x，sdk尚未适配
- AliceBot需要借助Mirai套件下运行，且其为一个独立的机器人

## 工程文件
- `Core`：核心功能类
- `Entity`：实体类，用于实例化参数
- `Extensions`：与Modules耦合的外部功能
- `Plugins`：机器人插件
- `Tools`：机器人可能会使用到的非核心的外部功能
- `Program.cs`：程序入口点
> 请注意设置.gitignore来排除.vs与.git参数文件以及/obj与/bin等输出文件夹

## 分支信息
| 分支名 | 功能 |
| ---- | ---- |
| master | 主分支 |
| develop | 开发分支 |
| canary | 测试分支，用以测试开发分支将要加入的代码 | 

## 功能实现
- 多api色图|已完成
- osusig查询|已完成
- 简单的早晚安|已完成
- 群欢迎和禁言变动提示|已完成
- 随机东方图片|已完成
- 随机猫猫图|已完成
- osu查询|未完成
- 数据库使用|正在工作
- 随机禁言|未完成
- 异常抛出提醒|未完成
- MC查询|未完成
- 公告系统|未完成

## 关联项目
- [Mirai](https://github.com/mamoe/mirai)
- [Mirai Console](https://github.com/mamoe/mirai-console)
- [mirai-api-http](https://github.com/project-mirai/mirai-api-http)
- [Mirai CSharp](https://github.com/Executor-Cheng/Mirai-CSharp)
- [Lolicon API](https://api.lolicon.app/)
- [Elbot](https://github.com/YunYouJun/el-bot)

## 如何使用
- wiki还等待编写中

## 注意事项
- 一切仅供交流学习
- 请使用c# >= 8.0建构项目
- 请使用`develop`分支进行开发，`master`为稳定运行分支，请勿直接推送`master`分支commit
