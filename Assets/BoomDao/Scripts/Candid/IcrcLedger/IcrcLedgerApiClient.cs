using System;
using System.Buffers;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid;
using System.Threading.Tasks;
using Candid.IcrcLedger;
using EdjCase.ICP.Agent.Responses;
using System.Collections.Generic;
using UnityEngine;
using Tokens = EdjCase.ICP.Candid.Models.UnboundedUInt;

namespace Candid.IcrcLedger
{
	public class IcrcLedgerApiClient
	{
		public IAgent Agent { get; }

		public Principal CanisterId { get; }

		public CandidConverter? Converter { get; }

		public IcrcLedgerApiClient(IAgent agent, Principal canisterId, CandidConverter? converter = default)
		{
			this.Agent = agent;
			this.CanisterId = canisterId;
			this.Converter = converter;
		}

		public async Task<Models.GetTransactionsResponse> GetTransactions(Models.GetTransactionsRequest arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "get_transactions", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.GetTransactionsResponse>(this.Converter);
		}

		public async Task<Models.GetBlocksResponse> GetBlocks(Models.GetBlocksArgs arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "get_blocks", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.GetBlocksResponse>(this.Converter);
		}

		public async Task<Models.DataCertificate> GetDataCertificate()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "get_data_certificate", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.DataCertificate>(this.Converter);
		}

		public async Task<string> Icrc1Name()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_name", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<string> Icrc1Symbol()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_symbol", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<byte> Icrc1Decimals()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_decimals", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<byte>(this.Converter);
		}

		public async Task<Dictionary<string, Models.MetadataValue>> Icrc1Metadata()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_metadata", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Dictionary<string, Models.MetadataValue>>(this.Converter);
		}

		public async Task<Tokens> Icrc1TotalSupply()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_total_supply", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Tokens>(this.Converter);
		}

		public async Task<Tokens> Icrc1Fee()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_fee", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Tokens>(this.Converter);
		}

		public async Task<OptionalValue<Models.Account>> Icrc1MintingAccount()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_minting_account", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<OptionalValue<Models.Account>>(this.Converter);
		}

		/*public async Task<Tokens> Icrc1BalanceOf(Models.Account arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_balance_of", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Tokens>(this.Converter);
		}*/
		
		public async Task<Tokens> Icrc1BalanceOf(Models.Account arg0)
		{
			//Extract the owner from the account.
			Principal owner = arg0.Owner;
			
			//Ensure that the subaccount property is not null
			//if it is null, substitute a new default instance which represents "none"
			var subaccountOptional = arg0.Subaccount ?? new Models.Account.SubaccountInfo();
			
			// If a value is provided, check if its length is exactly 32.
			// If not, then treat it as "none" by replacing with a default instance.
			if (subaccountOptional.HasValue)
			{
				var list = subaccountOptional.GetValueOrDefault();
				if (list == null || list.Count != 32)
				{
					subaccountOptional = new Models.Account.SubaccountInfo();
				}
			}
			
			// Build an anonymous object with properties that match the expected Candid record.
			// The property names ("owner" and "subaccount") must exactly match the canister's record.
			var accountRecord = new
			{
				owner = owner,
				subaccount = subaccountOptional // remains of type OptionalValue<List<byte>>
			};

			// Convert the anonymous object into a Candid argument.
			// This should serialize into a record { owner : principal; subaccount : opt blob }.
			CandidArg candidArg = CandidArg.FromCandid(
				CandidTypedValue.FromObject(accountRecord, this.Converter)
			);

			// Send the query to the canister.
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_balance_of", candidArg);
			CandidArg reply = response.ThrowOrGetReply();
			
			// Convert and return the result as a Tokens object.
			return reply.ToObjects<Tokens>(this.Converter);
		}

		public async Task<Models.TransferResult> Icrc1Transfer(Models.TransferArg arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAndWaitAsync(this.CanisterId, "icrc1_transfer", arg);
			return reply.ToObjects<Models.TransferResult>(this.Converter);
		}

		public async Task<List<Models.StandardRecord>> Icrc1SupportedStandards()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc1_supported_standards", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<List<Models.StandardRecord>>(this.Converter);
		}

		public async Task<Models.ApproveResult> Icrc2Approve(Models.ApproveArgs arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAndWaitAsync(this.CanisterId, "icrc2_approve", arg);
			return reply.ToObjects<Models.ApproveResult>(this.Converter);
		}

		public async Task<Models.Allowance> Icrc2Allowance(Models.AllowanceArgs arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc2_allowance", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.Allowance>(this.Converter);
		}

		public async Task<Models.TransferFromResult> Icrc2TransferFrom(Models.TransferFromArgs arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAndWaitAsync(this.CanisterId, "icrc2_transfer_from", arg);
			return reply.ToObjects<Models.TransferFromResult>(this.Converter);
		}
	}
}