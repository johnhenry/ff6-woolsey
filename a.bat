@ff6t_translator
@IF %ERRORLEVEL% NEQ 0 ( exit /b )
@xkas -o ff6t-english.smc _asm/_main.asm
@xkas -o ff6t-ex-english.smc _asm/_main.asm
@del _script-check.tmp 2>nul