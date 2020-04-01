﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Managers.SubManagers.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.SubManagers
{
  internal class AccountManager : IAccountManager
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AccountManager(
      in IAccountingDataAccess accountingDataAccess,
      in IMapper mapper,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(_accountingDataAccess));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccounts()
    {
      _logger.LogVerbose("Request received for all accounts.");

      IEnumerable<Account> accounts = await _accountingDataAccess.GetAllAccounts();

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(accounts);

      return accountDtos;
    }

    public async Task<AddAccountResult> AddAccount(AccountDto accountDto)
    {
      _logger.LogVerbose($"Add account request received, account details: {accountDto}");

      if (accountDto == null)
      {
        return AddAccountResult.CreateFailure($"Argument \"{nameof(accountDto)}\" cannot be null.");
      }

      Account account = _mapper.Map<AccountDto, Account>(accountDto);

      try
      {
        await _accountingDataAccess.AddAccount(account);
      }
      catch (AccountAlreadyExistsException ex)
      {
        _logger.LogError($"Failed to add account, already exists: \"{ex.Message}\".");

        return AddAccountResult.CreateFailure(ex.Message);
      }

      _logger.LogInformation($"Account added: {account}");

      return AddAccountResult.CreateSuccess();
    }
  }
}
