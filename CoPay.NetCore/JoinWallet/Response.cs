using System;

namespace CoPay.JoinWallet
{
    public class Response
    {
        public String copayerId { get; set; }

        // TODO, whenever we need this, grab from JSON schema below
        // public String wallet { get; set; }
    }
}

// {
// 	"copayerId": "0d41590111346bc94a7ef663dbcd0dcc6f1f727bc298fac320312bbb15af6c55",
// 	"wallet": {
// 		"version": "1.0.0",
// 		"createdOn": 1506905755,
// 		"id": "24bdfed2-b255-484b-a0f7-3c598af9ea72",
// 		"name": "gus",
// 		"m": 3,
// 		"n": 3,
// 		"singleAddress": false,
// 		"status": "complete",
// 		"publicKeyRing": [
// 			{
// 				"xPubKey": "xpub661MyMwAqRbcEcoTJd2oQbtXi55zo6xYeaQb8eFCHuSPMC6SCPx9zRDnae6kghRVsGJgqrNTYTUfP5kHXXV1nje8ZGRCQixfB6DRW8nrAXf",
// 				"requestPubKey": "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93"
// 			},
// 			{
// 				"xPubKey": "xpub661MyMwAqRbcFAeou6Np457xqg6gToT5WjzFd8jRRmdbMbNky45svH1KPQpgmaSJyuJvr9WMDiC8iGZwGf1dxbbvr6AjYyb25tV2ZQ2xvLV",
// 				"requestPubKey": "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93"
// 			},
// 			{
// 				"xPubKey": "xpub661MyMwAqRbcEp8W9hNpyy5XyNcxEzeHZEoT4kD8DiBVSSg3w8K4aBWggnGzU4nkTMBDKWQksaJ6zHvUBo6uyqL5YAyYnj933LZXbhdTi5f",
// 				"requestPubKey": "1GotV7sJshxLFDbUeBUHypiKZmLRhVpD3v"
// 			}
// 		],
// 		"copayers": [
// 			{
// 				"version": 2,
// 				"createdOn": 1506906134,
// 				"coin": "btc",
// 				"id": "9891df433b19d13f0efa1acec55213f528badb84b5336db4bb212994d031ad19",
// 				"name": "gus3",
// 				"xPubKey": "xpub661MyMwAqRbcEcoTJd2oQbtXi55zo6xYeaQb8eFCHuSPMC6SCPx9zRDnae6kghRVsGJgqrNTYTUfP5kHXXV1nje8ZGRCQixfB6DRW8nrAXf",
// 				"requestPubKey": "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93",
// 				"signature": "3045022100f91274a723ac5f0375224856950249bd24cda1cdbfbda71cf94492f6043f9e9d022014fd1994cc30b2de3f54d77dd3641548b3a563cbc1a3dde2ff5d77f94dd42264",
// 				"requestPubKeys": [
// 					{
// 						"key": "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93",
// 						"signature": "3045022100f91274a723ac5f0375224856950249bd24cda1cdbfbda71cf94492f6043f9e9d022014fd1994cc30b2de3f54d77dd3641548b3a563cbc1a3dde2ff5d77f94dd42264"
// 					}
// 				],
// 				"customData": null
// 			},
// 			{
// 				"version": 2,
// 				"createdOn": 1506906605,
// 				"coin": "btc",
// 				"id": "356c335fa1109f861b2160d44b8a810c4be2b30040f36c1c9930931ced3644a4",
// 				"name": "gus3",
// 				"xPubKey": "xpub661MyMwAqRbcFAeou6Np457xqg6gToT5WjzFd8jRRmdbMbNky45svH1KPQpgmaSJyuJvr9WMDiC8iGZwGf1dxbbvr6AjYyb25tV2ZQ2xvLV",
// 				"requestPubKey": "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93",
// 				"signature": "304402200742373a0a112adb4aad5983ce6d4790b1c402e35921185c5d184a2262151109022074514ac986870c8b17b82060ed3bdf80b361199c0cccc93dd0f580fec20d8362",
// 				"requestPubKeys": [
// 					{
// 						"key": "03d222074bb076cf3b0b47e8a562d106cc833e39d95f1479600faeaed702613e93",
// 						"signature": "304402200742373a0a112adb4aad5983ce6d4790b1c402e35921185c5d184a2262151109022074514ac986870c8b17b82060ed3bdf80b361199c0cccc93dd0f580fec20d8362"
// 					}
// 				],
// 				"customData": null
// 			},
// 			{
// 				"version": 2,
// 				"createdOn": 1506915374,
// 				"coin": "btc",
// 				"xPubKey": "xpub661MyMwAqRbcEp8W9hNpyy5XyNcxEzeHZEoT4kD8DiBVSSg3w8K4aBWggnGzU4nkTMBDKWQksaJ6zHvUBo6uyqL5YAyYnj933LZXbhdTi5f",
// 				"id": "0d41590111346bc94a7ef663dbcd0dcc6f1f727bc298fac320312bbb15af6c55",
// 				"name": "gus3",
// 				"requestPubKey": "1GotV7sJshxLFDbUeBUHypiKZmLRhVpD3v",
// 				"signature": "3044022050ca3496301a4dd5e85db34745e3ffec2c15f381fba0b0fb22e61dbe3bd5f76502204fbcdeccb77a0d603752dba0b6b235fd42ba8ef7c897a56c2354ca3d97e33fab",
// 				"requestPubKeys": [
// 					{
// 						"key": "1GotV7sJshxLFDbUeBUHypiKZmLRhVpD3v",
// 						"signature": "3044022050ca3496301a4dd5e85db34745e3ffec2c15f381fba0b0fb22e61dbe3bd5f76502204fbcdeccb77a0d603752dba0b6b235fd42ba8ef7c897a56c2354ca3d97e33fab"
// 					}
// 				]
// 			}
// 		],
// 		"pubKey": "0254fea7b08745c15103765a5c299b354ac6fbd3fa6a33c5ee84b6fa0fd108ab4e",
// 		"coin": "btc",
// 		"network": "livenet",
// 		"derivationStrategy": "BIP44",
// 		"addressType": "P2SH",
// 		"addressManager": {
// 			"version": 2,
// 			"derivationStrategy": "BIP44",
// 			"receiveAddressIndex": 0,
// 			"changeAddressIndex": 0,
// 			"copayerIndex": 2147483647
// 		},
// 		"scanStatus": null
// 	}
// }