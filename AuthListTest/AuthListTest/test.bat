REM warm up
AuthListTest -ITERATIONS 10 -THREADS 100 -SILENT 
REM test

AuthListTest -ITERATIONS 500 -THREADS 1		%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 10	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 20	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 30	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 40	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 50	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 60	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 70	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 80	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 90	%1 -SILENT 
AuthListTest -ITERATIONS 500 -THREADS 100	%1 -SILENT 