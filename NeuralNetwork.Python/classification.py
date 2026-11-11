# coding: UTF-8
import math
import matplotlib.pyplot as plt

# シグモイド関数
def sigmoid(x):
    # 任意の入力を 0～1 の範囲に写し、ニューロンの出力として使う。
    return 1.0 / (1.0 + math.exp(-x))

# ニューロン
class Neuron:
    # 入力の加重和を蓄積し、出力を活性化関数で求める。
    input_sum = 0.0
    output = 0.0

    def setInput(self, inp):
        self.input_sum += inp

    def getOutput(self):
        self.output = sigmoid(self.input_sum)
        return self.output

    def reset(self):
        self.input_sum = 0.0
        self.output = 0.0

# ニューラルネットワーク
class NeuralNetwork:
    # 2 個の座標入力とバイアスを、2 個の中間ニューロンへ接続する重み。
    w_im = [[0.5, 0.5], [-0.5, 1.0], [0.5, -0.5]]
    # 2 個の中間出力とバイアスを、1 個の出力ニューロンへ接続する重み。
    w_mo = [0.1, -0.5, 0.200]

    # 入力層と中間層の末尾に定数 1.0 を置き、バイアス入力として使う。
    input_layer = [0.0, 0.0, 1.0]
    middle_layer = [Neuron(), Neuron(), 1.0]
    output_layer = Neuron()

    # 1 件の座標を順方向に伝播し、分類スコアを返す。
    def commit(self, input_data):
        # 前回の推論で残った加重和を消してから、今回の入力を設定する。
        self.input_layer[0] = input_data[0]
        self.input_layer[1] = input_data[1]

        self.middle_layer[0].reset()
        self.middle_layer[1].reset()

        self.output_layer.reset()

        # 各入力とバイアスを対応する重みで掛け、中間層へ加算する。
        self.middle_layer[0].setInput(self.input_layer[0] * self.w_im[0][0])
        self.middle_layer[0].setInput(self.input_layer[1] * self.w_im[1][0])
        self.middle_layer[0].setInput(self.input_layer[2] * self.w_im[2][0])

        self.middle_layer[1].setInput(self.input_layer[0] * self.w_im[0][1])
        self.middle_layer[1].setInput(self.input_layer[1] * self.w_im[1][1])
        self.middle_layer[1].setInput(self.input_layer[2] * self.w_im[2][1])

        # 中間層の出力とバイアスを出力ニューロンへ伝える。
        self.output_layer.setInput(self.middle_layer[0].getOutput() * self.w_mo[0])
        self.output_layer.setInput(self.middle_layer[1].getOutput() * self.w_mo[1])
        self.output_layer.setInput(self.middle_layer[2] * self.w_mo[2])

        return self.output_layer.getOutput()

    def learn(self, input_data):
        # まず現在の重みで予測し、教師ラベルとの差から更新量を求める。
        output_data = self.commit([input_data[0], input_data[1]])
        correct_value = input_data[2]
        # 学習率は各更新量に掛け、重みが変化する幅を調整する。
        k = 0.3

        # 出力層→中間層
        # δmo = (正解値 - 出力値) × 出力の微分
        dalta_w_mo = (correct_value - output_data) * output_data * (1.0 - output_data)
        # 中間層へ誤差を戻す計算には、更新前の出力層の重みを使う。
        old_w_mo = list(self.w_mo)
        # 修正量 = δmo × 中間層の値 × 学習係数
        self.w_mo[0] += self.middle_layer[0].output * dalta_w_mo * k
        self.w_mo[1] += self.middle_layer[1].output * dalta_w_mo * k
        self.w_mo[2] += self.middle_layer[2] * dalta_w_mo * k

        # 中間層→入力層
        # δim = δmo × 中間出力の重み × 中間層の微分
        delta_w_im = [
            dalta_w_mo * old_w_mo[0] * self.middle_layer[0].output * (1.0 - self.middle_layer[0].output),
            dalta_w_mo * old_w_mo[1] * self.middle_layer[1].output * (1.0 - self.middle_layer[1].output)
        ]
        # 修正量 = δim × 入力層の値 × 学習係数
        self.w_im[0][0] += self.input_layer[0] * delta_w_im[0] * k
        self.w_im[0][1] += self.input_layer[0] * delta_w_im[1] * k
        self.w_im[1][0] += self.input_layer[1] * delta_w_im[0] * k
        self.w_im[1][1] += self.input_layer[1] * delta_w_im[1] * k
        self.w_im[2][0] += self.input_layer[2] * delta_w_im[0] * k
        self.w_im[2][1] += self.input_layer[2] * delta_w_im[1] * k

# 座標の桁を小さくするための基準点と倍率。学習時と推論時で同じ変換を使う。
base_x = 34.2
base_y = 135.1
base_rate = 0.25

# 教師データの座標を基準点からの差にし、ラベルを整数にそろえる。
def to_train(data):
    return [(float(data[0]) - base_x) * base_rate, (float(data[1]) - base_y) * base_rate, int(data[2])]

# テスト座標にも教師データと同じ尺度変換を適用する。
def to_test(data):
    return [(float(data[0]) - base_x) * base_rate, (float(data[1]) - base_y) * base_rate]

# 表示用に、学習時の尺度変換を元の座標へ戻す。
def from_train(data):
    return [data[0] / base_rate + base_x, data[1] / base_rate + base_y]

# CSV の各行を座標・正解ラベルに分け、学習用の尺度へ変換する。
training_data = []
training_data_file = open("./data/locations.training.csv", "r")
for line in training_data_file:
    data = line.rstrip().split(",")
    training_data.append(to_train(data))
training_data_file.close()

#print(training_data)

# ニューラルネットワーク
neural_network = NeuralNetwork()

# 教師データを 1000 周し、各サンプルのたびに重みを更新する。
#print(neural_network.w_im)
#print(neural_network.w_mo)

for t in range(0, 1000):
    for data in training_data:
        neural_network.learn(data)

#print(neural_network.w_im)
#print(neural_network.w_mo)

# 学習に使っていない座標を、推論結果の確認に使う。
test_data = [[34.6, 138.0], [34.6, 138.18], [35.4, 138.0], [34.98, 138.1], [35.0, 138.25], [35.4, 137.6], [34.98, 137.52], [34.5, 138.5], [35.4, 138.1]]

for index in range(len(test_data)):
    test_data[index] = to_test(test_data[index])    

#print(test_data)

# 予測値を 0.5 で分け、福井とそれ以外の座標を別々に蓄積する。
position_in_fukui_result = [[], []]
position_out_fukui_result = [[], []]

for data in test_data:
    position = from_train(data)
    if neural_network.commit(data) < 0.5:
        position_in_fukui_result[0].append(position[1])
        position_in_fukui_result[1].append(position[0])
    else:
        position_out_fukui_result[0].append(position[1])
        position_out_fukui_result[1].append(position[0])

# 教師ラベルで学習データを分け、予測結果と同じ形式で表示できるようにする。
position_in_fukui_training = [[], []]
position_out_fukui_training = [[], []]
for data in training_data:
    position = from_train(data)
    if data[2] < 0.5:
        position_in_fukui_training[0].append(position[1])
        position_in_fukui_training[1].append(position[0])
    else:
        position_out_fukui_training[0].append(position[1])
        position_out_fukui_training[1].append(position[0])

# 教師データは +、推論結果は ○ で描き、正解ラベルと予測を比較する。
plt.scatter(position_in_fukui_training[0], position_in_fukui_training[1], c="red", label="Train - Fukui", marker="+")
plt.scatter(position_out_fukui_training[0], position_out_fukui_training[1], c="blue", label="Train - Others", marker="+")
plt.scatter(position_in_fukui_result[0], position_in_fukui_result[1], c="red", label="Result - Fukui", marker="o")
plt.scatter(position_out_fukui_result[0], position_out_fukui_result[1], c="blue", label="Result - Others", marker="o")
plt.legend()
plt.show()
