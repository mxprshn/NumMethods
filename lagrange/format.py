

def format_float(value):
    return "{:.20f}".format(value)


def format_float_table(table, header_0, header_1):
    result = "\n|{:_^30}".format(header_0) + "|" + "{:_^30}|\n".format(header_1)
    for first, second in table:
        result += ("|{:^30.15f}".format(first) + "|" + "{:^30.15f}|\n".format(second))
    return result


def format_error(message):
    return "ОШИБКА: " + message
