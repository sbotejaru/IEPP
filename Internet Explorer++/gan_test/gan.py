#
# from https://realpython.com/generative-adversarial-networks/
#
import torch
from torch import nn

import math
import matplotlib.pyplot as plt
import numpy as np


def prepare_train_data():
    # train data is a set of points in 2D plane
    # that makes up a sinusoid between [-pi, pi]
    train_data_length = 1024
    train_data = torch.zeros((train_data_length, 2))
    train_data[:, 0] = 2 * math.pi * torch.rand(train_data_length) - math.pi
    train_data[:, 1] = torch.sin(train_data[:, 0])
    train_labels = torch.zeros(train_data_length)

    return [(train_data[i].cuda(), train_labels[i].cuda()) for i in range(train_data_length)]


class Discriminator(nn.Module):
    def __init__(self):
        super().__init__()

        # the discriminator model is made of 4 fully-connected layers
        # the input is a 2D point and the output is a single dimension (a scalar)

        # the idea of the discriminator is to map the 2D to 1D, that is,
        # to recognize whether that point is a real sample or a fake one (generated)
        self.model = nn.Sequential(
            nn.Linear(2, 256),
            nn.ReLU(),
            nn.Dropout(0.3),
            nn.Linear(256, 128),
            nn.ReLU(),
            nn.Dropout(0.3),
            nn.Linear(128, 64),
            nn.ReLU(),
            nn.Dropout(0.3),
            nn.Linear(64, 1),
            nn.Sigmoid(),
        )

    def forward(self, x):
        output = self.model(x)
        return output


class Generator(nn.Module):
    def __init__(self):
        super().__init__()

        # the generator takes a float number and transforms it into a 2D point on the sinusoid
        self.model = nn.Sequential(
            nn.Linear(1, 16),
            nn.ReLU(),
            nn.Linear(16, 32),
            nn.ReLU(),
            nn.Linear(32, 2),
        )

    def forward(self, x):
        output = self.model(x)
        return output


def train_model(
        generator: Generator,
        discriminator: Discriminator,
        train_loader: torch.utils.data.DataLoader,
        model_path: str):
    lr = 0.001
    num_epochs = 300
    loss_function = nn.BCELoss()
    loss_function = loss_function.cuda()
    generator = generator.cuda()
    discriminator = discriminator.cuda()

    optimizer_discriminator = torch.optim.Adam(discriminator.parameters(), lr=lr)
    optimizer_generator = torch.optim.Adam(generator.parameters(), lr=lr)

    batch_size = len(train_loader.dataset) // len(train_loader)
    print(f'batch_size: {batch_size}')

    for epoch in range(num_epochs):
        for n, (real_samples, _) in enumerate(train_loader):
            # the latent space is a 1D space with random numbers from a normal distribution in N(0, 1)
            latent_space_samples = torch.randn((batch_size, 1)).cuda()

            # generator takes latent space samples and computes 2D points;
            # all thus generated 2D points are labeled with 0, since they are not real, but generated samples
            generated_samples = generator(latent_space_samples)            
            generated_samples_labels = torch.zeros((batch_size, 1)).cuda()

            # the real samples are made of the train data, that is 2D points on the sinusoid
            # they are labeled with 1, since they make up the real data
            # we put together the fake data made up by generator with the real data
            real_samples_labels = torch.ones((batch_size, 1)).cuda()
            all_samples = torch.cat((real_samples, generated_samples)).cuda()
            all_samples_labels = torch.cat((real_samples_labels, generated_samples_labels)).cuda()

            # the idea of training the discriminator is that this learns to discern
            # between the real data and the fake data
            discriminator.zero_grad()
            output_discriminator = discriminator(all_samples)
            loss_discriminator = loss_function(output_discriminator, all_samples_labels)
            loss_discriminator.backward()
            optimizer_discriminator.step()

            # we generate another latent data for training the generator
            latent_space_samples = torch.randn((batch_size, 1)).cuda()

            # training the generator means that everything that comes from the generator
            # is recognized as real data; since the discriminator is frozen, if the output
            # is more close to zero than one, it will force the generator to readjust to
            # produce samples closer to the real ones
            generator.zero_grad()
            generated_samples = generator(latent_space_samples)
            output_discriminator_generated = discriminator(generated_samples)
            loss_generator = loss_function(output_discriminator_generated, real_samples_labels)
            loss_generator.backward()
            optimizer_generator.step()

            # show loss
            if epoch % 50 == 0 and n == batch_size - 1:
                print(f"Epoch: {epoch} Loss D.: {loss_discriminator:.6f} Loss G.: {loss_generator:.6f}")

            d = {'D': discriminator, 'G': generator}
            torch.save(d, model_path)


if __name__ == "__main__":
    torch.manual_seed(111)

    MODEL_PATH = 'sin-gan-snapshot2.pickle'

    do_train = False

    crt_device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")   
    print(crt_device)

    # generator=Generator().to(crt_device)
    # discriminator=Discriminator().to(crt_device)

    # generator = generator.cuda()
    # discriminator = discriminator.cuda()

    if do_train:
        train_set = prepare_train_data()
        train_data = torch.stack([x[0] for x in train_set])

        # fig, ax = plt.subplots(1, 1, figsize=(10, 5))
        # ax.scatter(train_data[:, 0], train_data[:, 1])
        # ax.grid(); plt.show()
        
        train_data = train_data.cuda()       

        train_loader = torch.utils.data.DataLoader(
            train_set, batch_size=32, shuffle=True
        )

        train_model(
            generator=Generator(),
            discriminator=Discriminator(),
            train_loader=train_loader,
            model_path=MODEL_PATH)

        print(f"Model trained.")
        exit()

    d = torch.load(MODEL_PATH)
    discriminator, generator = d['D'], d['G']

    latent_space_samples = torch.randn(1000, 1).cuda()
    generated_samples = generator(latent_space_samples).detach()

    fig, ax = plt.subplots(1, 1, figsize=(10, 5))
    cm = plt.cm.get_cmap('jet')
    sc = ax.scatter(
        generated_samples[:, 0].cpu().numpy(),
        generated_samples[:, 1].cpu().numpy(),
        c=np.concatenate(latent_space_samples.cpu().numpy()),
        cmap=cm)
    #fig.colorbar(sc, ax=ax).set_label('latent_space_samples')
    #ax.grid()
    plt.axis('off')
    plt.savefig('output.png', bbox_inches='tight')