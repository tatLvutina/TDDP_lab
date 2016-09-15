# -*- coding: utf-8  -*-
import random as R
from math import *

def monte_carlo(fun, Low, Up, N):
    s=0
    for i in range(1,N):
        x=Low+(Up-Low)*R.random()
        s=s+eval(fun)
    return ((Up-Low)*s)/N


def param_enter():
    global fun
    global Low,Up,step,N

    print("Интеграл берется по dx")

    fun = input('''\nВНИМАНИЕ! Математическая нотация Python\n
    Для возведения в степень вместо ^ используется **\n
    Введите подынтегральную функцию, заключенную в кавычки\n
    Например, "x**2":     ''')
    Low = int(input("Нижний предел интегрирования = "))
    Up = int(input("Верхний предел интегрирования = "))
    N = int(input("Точность (кол-во бросаемых точек, например, 10000) = "))

    #отрезок для подсчета одним клиентом
    step = (Up-Low)//2




if __name__ == '__main__':
    #код этого блока будет выполнен только если этот модуль
    #будет запущен как отдельный файл
    print("Модуль запущен как отдельный файл!\nКавычки при вводе не нужны!")
    param_enter()

    I = monte_carlo(fun, Low, Up, N)

    print(I)


