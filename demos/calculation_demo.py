import tp_unity_ai.tp_unity_ai as tp


def interface():
    num1 = 100
    num2 = 50
    addition = tp.calc.add_num(num1, num2)
    subtraction = tp.calc.sub_num(num1, num2)

    print(f'tp.add_sum(): {num1} + {num2} = {addition}')
    print(f'tp.sub_sum(): {num1} - {num2} = {subtraction}')


if __name__ == '__main__':
    interface()
